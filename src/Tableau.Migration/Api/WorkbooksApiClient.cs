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
using Tableau.Migration.Net.Rest.Sorting;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class WorkbooksApiClient :
        ContentApiClientBase, IWorkbooksApiClient
    {
        private readonly IContentFileStore _fileStore;
        private readonly IWorkbookPublisher _workbookPublisher;
        private readonly IConnectionManager _connectionManager;
        private readonly IConfigReader _configReader;

        public WorkbooksApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            IPermissionsApiClientFactory permissionsClientFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IContentReferenceFinderFactory finderFactory,
            IContentFileStore fileStore,
            IWorkbookPublisher workbookPublisher,
            ITagsApiClientFactory tagsClientFactory,
            IViewsApiClientFactory viewsClientFactory,
            IConnectionManager connectionManager,
            IConfigReader configReader)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _fileStore = fileStore;
            _workbookPublisher = workbookPublisher;
            Permissions = permissionsClientFactory.Create(this);
            Tags = tagsClientFactory.Create(this);
            Views = viewsClientFactory.Create();
            _connectionManager = connectionManager;
            _configReader = configReader;
        }

        #region - IPermissionsContentApiClientImplementation -

        /// <inheritdoc />
        public IPermissionsApiClient Permissions { get; }

        #endregion

        #region - ITagsContentApiClient Implementation -

        /// <inheritdoc />
        public ITagsApiClient Tags { get; }

        #endregion

        #region - IViewsContentApiClient Implementation -

        /// <inheritdoc />
        public IViewsApiClient Views { get; }

        #endregion

        /// <inheritdoc />
        public async Task<IPagedResult<IWorkbook>> GetAllWorkbooksAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
        {
            var getAllResult = await RestRequestBuilderFactory
                .CreateUri(UrlPrefix)
                .WithPage(pageNumber, pageSize)
                .WithSorts(new Sort("size", false))
                .ForGetRequest()
                .SendAsync<WorkbooksResponse>(cancel)
                .ToPagedResultAsync(async (r, c) =>
                {
                    // Take all items.
                    var results = ImmutableArray.CreateBuilder<IWorkbook>(r.Items.Length);

                    foreach (var item in r.Items)
                    {
                        // Convert them all to type Workbook.
                        if (item.Project is not null) // Project is null if item is in a personal space
                        {
                            var project = await FindProjectAsync(item, false, c).ConfigureAwait(false);
                            var owner = await FindOwnerAsync(item, false, c).ConfigureAwait(false);

                            if (project is null || owner is null)
                            {
                                Logger.LogWarning(
                                    SharedResourcesLocalizer[SharedResourceKeys.WorkbookSkippedMissingReferenceWarning],
                                    item.Id,
                                    item.Name,
                                    item.Project!.Id,
                                    project is null ? SharedResourcesLocalizer[SharedResourceKeys.NotFound] : SharedResourcesLocalizer[SharedResourceKeys.Found],
                                    item.Owner!.Id,
                                    owner is null ? SharedResourcesLocalizer[SharedResourceKeys.NotFound] : SharedResourcesLocalizer[SharedResourceKeys.Found]);
                                continue;
                            }

                            results.Add(new Workbook(item, project, owner));
                        }
                    }

                    // Produce immutable list of type IWorkbook and return.
                    return (IImmutableList<IWorkbook>)results.ToImmutable();
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return getAllResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IWorkbookDetails>> GetWorkbookAsync(Guid workbookId, CancellationToken cancel)
        {
            var getResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{workbookId.ToUrlSegment()}")
                .ForGetRequest()
                .SendAsync<WorkbookResponse>(cancel)
                .ToResultAsync(async (response, cancel) =>
                {
                    var workbook = Guard.AgainstNull(response.Item, () => response.Item);

                    var project = await FindProjectAsync(workbook, true, cancel).ConfigureAwait(false);
                    var owner = await FindOwnerAsync(workbook, true, cancel).ConfigureAwait(false);

                    return (IWorkbookDetails)new WorkbookDetails(workbook, project, owner);
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return getResult;
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposableResult<FileDownload>> DownloadWorkbookAsync(
            Guid workbookId,
            CancellationToken cancel)
        {
            var downloadResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{workbookId.ToUrlSegment()}/{RestUrlPrefixes.Content}")
                .WithQuery("includeExtract", _configReader.Get<IWorkbook>().IncludeExtractEnabled.ToString())
                .ForGetRequest()
                .DownloadAsync(cancel)
                .ConfigureAwait(false);

            return downloadResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IWorkbookDetails>> PublishWorkbookAsync(
            IPublishWorkbookOptions options,
            CancellationToken cancel)
            => await _workbookPublisher.PublishAsync(options, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IResult<IWorkbookDetails>> PublishAsync(IPublishableWorkbook item, CancellationToken cancel)
        {
            var fileStream = await item.File.OpenReadAsync(cancel).ConfigureAwait(false);
            await using (fileStream)
            {
                var publishOptions = new PublishWorkbookOptions(item, fileStream.Content);
                var publishResult = await PublishWorkbookAsync(publishOptions, cancel)
                    .ConfigureAwait(false);

                return publishResult;
            }
        }

        /// <inheritdoc />
        public async Task<IResult<IUpdateWorkbookResult>> UpdateWorkbookAsync(
            Guid workbookId,
            CancellationToken cancel,
            string? newName = null,
            string? newDescription = null,
            Guid? newProjectId = null,
            Guid? newOwnerId = null,
            bool? newShowTabs = null,
            bool? newRecentlyViewed = null,
            bool? newEncryptExtracts = null)
        {
            var updateResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{workbookId.ToUrlSegment()}")
                .ForPutRequest()
                .WithXmlContent(
                    new UpdateWorkbookRequest(
                        newName,
                        newDescription,
                        newProjectId,
                        newOwnerId,
                        newShowTabs,
                        newRecentlyViewed,
                        newEncryptExtracts))
                .SendAsync<UpdateWorkbookResponse>(cancel)
                .ToResultAsync<UpdateWorkbookResponse, IUpdateWorkbookResult>(r => new UpdateWorkbookResult(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return updateResult;
        }

        #region - IApiPageAccessor<IWorkbook> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IWorkbook>> GetPageAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
            => await GetAllWorkbooksAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        #endregion

        #region - IPagedListApiClient<IWorkbook> Implementation -

        /// <inheritdoc />
        public IPager<IWorkbook> GetPager(int pageSize) => new ApiListPager<IWorkbook>(this, pageSize);

        #endregion

        #region - IPullApiClient<IWorkbook, IPublishableWorkbook> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IPublishableWorkbook>> PullAsync(
            IWorkbook contentItem,
            CancellationToken cancel)
        {
            var connectionsResult = await GetConnectionsAsync(contentItem.Id, cancel).ConfigureAwait(false);
            if (!connectionsResult.Success)
            {
                return connectionsResult.CastFailure<IPublishableWorkbook>();
            }

            var downloadResult = await DownloadWorkbookAsync(contentItem.Id, cancel).ConfigureAwait(false);
            if (!downloadResult.Success)
            {
                return downloadResult.CastFailure<IPublishableWorkbook>();
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
                var workbookResult = await file.DisposeOnThrowOrFailureAsync(
                    async () => await GetWorkbookAsync(contentItem.Id, cancel).ConfigureAwait(false)
                ).ConfigureAwait(false);

                if (!workbookResult.Success)
                {
                    return workbookResult.CastFailure<IPublishableWorkbook>();
                }

                var publishWorkbook = new PublishableWorkbook(workbookResult.Value, connectionsResult.Value, file);

                return Result<IPublishableWorkbook>.Succeeded(publishWorkbook);
            }
        }

        #endregion

        #region - IOwnershipApiClient Implementation -

        /// <inheritdoc />
        public async Task<IResult> ChangeOwnerAsync(
            Guid workbookId,
            Guid newOwnerId,
            CancellationToken cancel)
        {
            var result = await UpdateWorkbookAsync(workbookId, cancel, newOwnerId: newOwnerId)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region - IConnectionsApiClient Implementation -

        /// <inheritdoc/>
        public async Task<IResult<ImmutableList<IConnection>>> GetConnectionsAsync(
            Guid workbookId,
            CancellationToken cancel)
            => await _connectionManager.ListConnectionsAsync(
                UrlPrefix,
                workbookId,
                cancel)
                .ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task<IResult<IConnection>> UpdateConnectionAsync(
            Guid workbookId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel)
            => await _connectionManager.UpdateConnectionAsync(
                UrlPrefix,
                workbookId,
                connectionId,
                options,
                cancel)
                .ConfigureAwait(false);

        #endregion
    }
}
