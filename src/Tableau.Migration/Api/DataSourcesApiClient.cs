//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Labels;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Net.Rest.Sorting;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class DataSourcesApiClient :
        ContentApiClientBase, IDataSourcesApiClient
    {
        private readonly IContentFileStore _fileStore;
        private readonly IDataSourcePublisher _dataSourcePublisher;
        private readonly IConnectionManager _connectionManager;
        private readonly IConfigReader _configReader;

        public DataSourcesApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            IPermissionsApiClientFactory permissionsClientFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IContentReferenceFinderFactory finderFactory,
            IContentFileStore fileStore,
            IDataSourcePublisher dataSourcePublisher,
            ITagsApiClientFactory tagsClientFactory,
            IConnectionManager connectionManager,
            ILabelsApiClientFactory labelsCLientFactory,
            IConfigReader configReader)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _fileStore = fileStore;
            _dataSourcePublisher = dataSourcePublisher;
            Labels = labelsCLientFactory.Create<IDataSource>();
            Permissions = permissionsClientFactory.Create(this);
            Tags = tagsClientFactory.Create(this);
            _connectionManager = connectionManager;
            _configReader = configReader;
        }

        #region - ILabelsContentApiClient Implementation -

        /// <inheritdoc />
        public ILabelsApiClient<IDataSource> Labels { get; }

        #endregion

        #region - IPermissionsContentApiClientImplementation -

        /// <inheritdoc />
        public IPermissionsApiClient Permissions { get; }

        #endregion

        #region - ITagsContentApiClient Implementation -

        /// <inheritdoc />
        public ITagsApiClient Tags { get; }

        #endregion

        /// <inheritdoc />
        public async Task<IPagedResult<IDataSource>> GetAllPublishedDataSourcesAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
        {
            var getAllResult = await RestRequestBuilderFactory
                .CreateUri(UrlPrefix)
                .WithPage(pageNumber, pageSize)
                .WithFilters(new Filter("isPublished", FilterOperator.Equal, "true"))
                .WithSorts(new Sort("size", false))
                .ForGetRequest()
                .SendAsync<DataSourcesResponse>(cancel)
                .ToPagedResultAsync(async (response, cancel) =>
                {
                    // Take all items.
                    var results = ImmutableArray.CreateBuilder<IDataSource>(response.Items.Length);

                    foreach (var item in response.Items)
                    {
                        // Convert them all to type DataSource.
                        if (item.Project is not null) // Project is null if item is in a personal space
                        {
                            var project = await FindProjectAsync(item, false, cancel).ConfigureAwait(false);
                            var owner = await FindOwnerAsync(item, false, cancel).ConfigureAwait(false);

                            if (project is null || owner is null)
                            {
                                Logger.LogWarning(
                                    SharedResourcesLocalizer[SharedResourceKeys.DataSourceSkippedMissingReferenceWarning],
                                    item.Id,
                                    item.Name,
                                    item.Project!.Id,
                                    project is null ? SharedResourcesLocalizer[SharedResourceKeys.NotFound] : SharedResourcesLocalizer[SharedResourceKeys.Found],
                                    item.Owner!.Id,
                                    owner is null ? SharedResourcesLocalizer[SharedResourceKeys.NotFound] : SharedResourcesLocalizer[SharedResourceKeys.Found]);
                                continue;
                            }

                            results.Add(new DataSource(item, project, owner));
                        }
                    }

                    // Produce immutable list of type IDataSource and return.
                    return (IImmutableList<IDataSource>)results.ToImmutable();
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return getAllResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IDataSourceDetails>> GetDataSourceAsync(Guid dataSourceId, CancellationToken cancel)
        {
            var getResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{dataSourceId.ToUrlSegment()}")
                .ForGetRequest()
                .SendAsync<DataSourceResponse>(cancel)
                .ToResultAsync(async (response, cancel) =>
                {
                    var dataSource = Guard.AgainstNull(response.Item, () => response.Item);

                    var project = await FindProjectAsync(response.Item, true, cancel).ConfigureAwait(false);
                    var owner = await FindOwnerAsync(response.Item, true, cancel).ConfigureAwait(false);

                    return (IDataSourceDetails)new DataSourceDetails(dataSource, project, owner);

                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return getResult;
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposableResult<FileDownload>> DownloadDataSourceAsync(Guid dataSourceId,
            CancellationToken cancel)
        {
            var downloadResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{dataSourceId.ToUrlSegment()}/{RestUrlPrefixes.Content}")
                .WithQuery("includeExtract", _configReader.Get<IDataSource>().IncludeExtractEnabled.ToString())
                .ForGetRequest()
                .DownloadAsync(cancel)
                .ConfigureAwait(false);

            return downloadResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IDataSourceDetails>> PublishDataSourceAsync(IPublishDataSourceOptions options, CancellationToken cancel)
            => await _dataSourcePublisher.PublishAsync(options, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IResult<IDataSourceDetails>> PublishAsync(IPublishableDataSource item, CancellationToken cancel)
        {
            var fileStream = await item.File.OpenReadAsync(cancel).ConfigureAwait(false);

            await using (fileStream)
            {
                var publishOptions = new PublishDataSourceOptions(item, fileStream.Content);
                var publishResult = await PublishDataSourceAsync(publishOptions, cancel)
                    .ConfigureAwait(false);

                return publishResult;
            }
        }

        /// <inheritdoc />
        public async Task<IResult<IUpdateDataSourceResult>> UpdateDataSourceAsync(
            Guid dataSourceId,
            CancellationToken cancel,
            string? newName = null,
            Guid? newProjectId = null,
            Guid? newOwnerId = null,
            bool? newCertification = null,
            string? newCertificationNote = null,
            bool? newEncryptExtracts = null)
        {
            var updateResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{dataSourceId.ToUrlSegment()}")
                .ForPutRequest()
                .WithXmlContent(
                    new UpdateDataSourceRequest(
                        newName,
                        newProjectId,
                        newOwnerId,
                        newCertification,
                        newCertificationNote,
                        newEncryptExtracts))
                .SendAsync<UpdateDataSourceResponse>(cancel)
                .ToResultAsync<UpdateDataSourceResponse, IUpdateDataSourceResult>(r => new UpdateDataSourceResult(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return updateResult;
        }

        #region - IApiPageAccessor<IDataSource> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IDataSource>> GetPageAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
            => await GetAllPublishedDataSourcesAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        #endregion

        #region - IPagedListApiClient<IDataSource> Implementation -

        /// <inheritdoc />
        public IPager<IDataSource> GetPager(int pageSize) => new ApiListPager<IDataSource>(this, pageSize);

        #endregion

        #region - IPullApiClient<IDataSource, IPublishableDataSource> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IPublishableDataSource>> PullAsync(
            IDataSource contentItem, CancellationToken cancel)
        {
            var connectionsResult = await GetConnectionsAsync(contentItem.Id, cancel).ConfigureAwait(false);
            if (!connectionsResult.Success)
            {
                return connectionsResult.CastFailure<IPublishableDataSource>();
            }

            var downloadResult = await DownloadDataSourceAsync(contentItem.Id, cancel).ConfigureAwait(false);
            if (!downloadResult.Success)
            {
                return downloadResult.CastFailure<IPublishableDataSource>();
            }

            await using (downloadResult)
            {
                var file = await _fileStore.CreateAsync(
                    contentItem,
                    downloadResult.Value,
                    cancel)
                    .ConfigureAwait(false);

                /* If we throw/fail (even from cancellation) before we can return the file handle,
                 * make sure the file is disposed. We clean up orphaned
                 * files at the end of the DI scope, but we don't want to 
                 * bloat disk usage when we're processing future pages of items.*/
                var dataSourceResult = await file.DisposeOnThrowOrFailureAsync( 
                    async () => await GetDataSourceAsync(contentItem.Id, cancel).ConfigureAwait(false)
                ).ConfigureAwait(false);

                if (!dataSourceResult.Success)
                {
                    return dataSourceResult.CastFailure<IPublishableDataSource>();
                }

                var publishDataSource = new PublishableDataSource(dataSourceResult.Value, connectionsResult.Value, file);

                return Result<IPublishableDataSource>.Succeeded(publishDataSource);
            }
        }

        #endregion

        #region - IOwnershipApiClient Implementation -

        public async Task<IResult> ChangeOwnerAsync(
            Guid dataSourceId,
            Guid newOwnerId,
            CancellationToken cancel)
        {
            var result = await UpdateDataSourceAsync(
                dataSourceId,
                cancel,
                newOwnerId: newOwnerId)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region - IConnectionsApiClient Implementation -

        /// <inheritdoc/>
        public async Task<IResult<ImmutableList<IConnection>>> GetConnectionsAsync(
            Guid dataSourceId,
            CancellationToken cancel)
            => await _connectionManager.ListConnectionsAsync(
                UrlPrefix,
                dataSourceId,
                cancel)
                .ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task<IResult<IConnection>> UpdateConnectionAsync(
            Guid dataSourceId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel)
            => await _connectionManager.UpdateConnectionAsync(
                UrlPrefix,
                dataSourceId,
                connectionId,
                options,
                cancel)
                .ConfigureAwait(false);

        #endregion
    }
}
