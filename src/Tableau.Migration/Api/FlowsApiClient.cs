//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.EmbeddedCredentials;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Paging;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class FlowsApiClient : ContentApiClientBase, IFlowsApiClient
    {
        private readonly IContentFileStore _fileStore;
        private readonly IFlowPublisher _flowPublisher;
        private readonly IConnectionManager _connectionManager;

        public FlowsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IContentFileStore fileStore,
            IFlowPublisher flowPublisher,
            IConnectionManager connectionManager,
            IPermissionsApiClientFactory permissionsClientFactory,
            IEmbeddedCredentialsApiClientFactory embeddedCredentialsApiClientFactory,
            ITagsApiClientFactory tagsClientFactory)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _fileStore = fileStore;
            _flowPublisher = flowPublisher;
            _connectionManager = connectionManager;
            Permissions = permissionsClientFactory.Create(this);
            EmbeddedCredentials = embeddedCredentialsApiClientFactory.Create(this);
            Tags = tagsClientFactory.Create(this);
        }

        #region - IPermissionsContentApiClient Implementation -

        /// <inheritdoc />
        public IPermissionsApiClient Permissions { get; }

        #endregion

        #region - IEmbeddedCredentialsContentApiClient Implementation -

        /// <inheritdoc />
        public IEmbeddedCredentialsApiClient EmbeddedCredentials { get; }

        #endregion

        #region - ITagsContentApiClient Implementation -

        /// <inheritdoc />
        public ITagsApiClient Tags { get; }

        #endregion

        private async Task<IPagedResult<IFlow>> GetAllFlowsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters, CancellationToken cancel)
        {
            var getAllResult = await RestRequestBuilderFactory
                .CreateUri(UrlPrefix)
                .WithPage(pageNumber, pageSize)
                .WithFilters(filters)
                .ForGetRequest()
                .SendAsync<FlowsResponse>(cancel)
                .ToPagedResultAsync(async (response, cancel) =>
                {
                    // Take all items.
                    var results = ImmutableArray.CreateBuilder<IFlow>(response.Items.Length);

                    foreach (var item in response.Items)
                    {
                        // Convert them all to type Flow.
                        if (item.Project is not null) // Project is null if item is in a personal space.
                        {
                            var project = await FindProjectAsync(item, false, cancel).ConfigureAwait(false);
                            var owner = await FindOwnerAsync(item, false, cancel).ConfigureAwait(false);

                            if (project is null || owner is null)
                                continue; //Warnings will be logged by prior method calls.

                            results.Add(new Flow(item, project, owner));
                        }
                    }

                    // Produce immutable list of type IFlow and return.
                    return (IImmutableList<IFlow>)results.ToImmutable();
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return getAllResult;
        }

        /// <summary>
        /// Downloads the prep flow file for the given ID.
        /// </summary>
        /// <param name="flowId">The ID to download the flow file for.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The file download result.</returns>
        public async Task<IAsyncDisposableResult<FileDownload>> DownloadFlowAsync(Guid flowId, CancellationToken cancel)
        {
            var downloadResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{flowId.ToUrlSegment()}/{RestUrlKeywords.Content}")
                .ForGetRequest()
                .DownloadAsync(cancel)
                .ConfigureAwait(false);

            return downloadResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IFlow>> PublishFlowAsync(IPublishFlowOptions options, CancellationToken cancel)
            => await _flowPublisher.PublishAsync(options, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IResult<IUpdateFlowResult>> UpdateFlowAsync(
            Guid flowId,
            CancellationToken cancel,
            Guid? newProjectId = null,
            Guid? newOwnerId = null)
        {
            var updateResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{flowId.ToUrlSegment()}")
                .ForPutRequest()
                .WithXmlContent(
                    new UpdateFlowRequest(
                        newProjectId,
                        newOwnerId))
                .SendAsync<UpdateFlowResponse>(cancel)
                .ToResultAsync<UpdateFlowResponse, IUpdateFlowResult>(r => new UpdateFlowResult(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return updateResult;
        }

        #region - IApiPageAccessor<IFlow> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IFlow>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllFlowsAsync(pageNumber, pageSize, [], cancel).ConfigureAwait(false);

        #endregion

        #region - IPagedListApiClient<IFlow> Implementation -

        /// <inheritdoc />
        public IPager<IFlow> GetPager(int pageSize) => new ApiListPager<IFlow>(this, pageSize);

        #endregion

        #region - IApiFilteredPageAccessor<IFlow> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IFlow>> GetPageAsync(IEnumerable<Filter> filters, int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllFlowsAsync(pageNumber, pageSize, filters, cancel).ConfigureAwait(false);

        #endregion

        #region - IFilteredPagedListApiClient<IFlow> Implementation -

        /// <inheritdoc />
        public IPager<IFlow> GetPager(IEnumerable<Filter> filters, int pageSize)
            => new ApiFilteredListPager<IFlow>(this, filters, pageSize);

        #endregion

        #region - INameSearchApiClient<IFlow> Implementation -

        /// <inheritdoc />
        FilterOperator INameSearchApiClient<IFlow>.NameFilterOperator { get; } = FilterOperator.Equal;

        #endregion

        #region - IReadApiClient<IFlowDetails> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IFlowDetails>> GetByIdAsync(Guid contentId, CancellationToken cancel)
        {
            var getResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{contentId.ToUrlSegment()}")
                .ForGetRequest()
                .SendAsync<FlowResponse>(cancel)
                .ToResultAsync(async (response, cancel) =>
                {
                    var flow = Guard.AgainstNull(response.Item, () => response.Item);

                    // Copy the flow output steps from the response to the flow item
                    flow.FlowOutputSteps = response.FlowOutputSteps;

                    var project = await FindProjectAsync(flow, true, cancel).ConfigureAwait(false);
                    var owner = await FindOwnerAsync(flow, true, cancel).ConfigureAwait(false);

                    return (IFlowDetails)new FlowDetails(flow, project, owner);
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return getResult;
        }

        #endregion

        #region - IPullApiClient<IFlow, IPublishableFlow> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IPublishableFlow>> PullAsync(IFlow contentItem, CancellationToken cancel)
        {
            var connectionsResult = await GetConnectionsAsync(contentItem.Id, cancel).ConfigureAwait(false);
            if (!connectionsResult.Success)
            {
                return connectionsResult.CastFailure<IPublishableFlow>();
            }

            var downloadResult = await DownloadFlowAsync(contentItem.Id, cancel).ConfigureAwait(false);
            if (!downloadResult.Success)
            {
                return downloadResult.CastFailure<IPublishableFlow>();
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
                var flowResult = await file.DisposeOnThrowOrFailureAsync(
                    async () => await GetByIdAsync(contentItem.Id, cancel).ConfigureAwait(false)
                ).ConfigureAwait(false);

                if (!flowResult.Success)
                {
                    return flowResult.CastFailure<IPublishableFlow>();
                }

                var publishableFlow = new PublishableFlow(flowResult.Value, connectionsResult.Value, file);

                return Result<IPublishableFlow>.Succeeded(publishableFlow);
            }
        }

        #endregion

        #region - IPublishApiClient<IPublishableFlow, IFlow> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IFlow>> PublishAsync(IPublishableFlow item, CancellationToken cancel)
            => await PublishFlowAsync(new PublishFlowOptions(item), cancel).ConfigureAwait(false);

        #endregion

        #region - IOwnershipApiClient Implementation -

        /// <inheritdoc />
        public async Task<IResult> ChangeOwnerAsync(
            Guid flowId,
            Guid newOwnerId,
            CancellationToken cancel)
        {
            var result = await UpdateFlowAsync(flowId, cancel, newOwnerId: newOwnerId)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region - IConnectionsApiClient Implementation -

        /// <inheritdoc/>
        public async Task<IResult<ImmutableList<IConnection>>> GetConnectionsAsync(
            Guid flowId,
            CancellationToken cancel)
            => await _connectionManager.ListConnectionsAsync(
                UrlPrefix,
                flowId,
                cancel)
                .ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task<IResult<IConnection>> UpdateConnectionAsync(
            Guid flowId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel)
            => await _connectionManager.UpdateConnectionAsync(
                UrlPrefix,
                flowId,
                connectionId,
                options,
                cancel)
                .ConfigureAwait(false);

        #endregion
    }
}
