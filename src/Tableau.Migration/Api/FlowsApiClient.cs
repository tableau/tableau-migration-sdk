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
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class FlowsApiClient : ContentApiClientBase, IFlowsApiClient
    {
        private readonly IContentFileStore _fileStore;
        private readonly IFlowPublisher _flowPublisher;

        public FlowsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory, 
            IContentReferenceFinderFactory finderFactory, 
            ILoggerFactory loggerFactory, 
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IContentFileStore fileStore,
            IFlowPublisher flowPublisher) 
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _fileStore = fileStore;
            _flowPublisher = flowPublisher;
        }

        /// <summary>
        /// Gets all prep flows in the current site.
        /// </summary>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>A list of a page of prep flows in the current site.</returns>
        public async Task<IPagedResult<IFlow>> GetAllFlowsAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            var getAllResult = await RestRequestBuilderFactory
                .CreateUri(UrlPrefix)
                .WithPage(pageNumber, pageSize)
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

                    // Produce immutable list of type IDataSource and return.
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
                .CreateUri($"{UrlPrefix}/{flowId.ToUrlSegment()}/{RestUrlPrefixes.Content}")
                .ForGetRequest()
                .DownloadAsync(cancel)
                .ConfigureAwait(false);

            return downloadResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IFlow>> PublishFlowAsync(IPublishFlowOptions options, CancellationToken cancel)
            => await _flowPublisher.PublishAsync(options, cancel).ConfigureAwait(false);

        #region - IApiPageAccessor<IFlow> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IFlow>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllFlowsAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        #endregion

        #region - IPagedListApiClient<IFlow> Implementation -

        /// <inheritdoc />
        public IPager<IFlow> GetPager(int pageSize) => new ApiListPager<IFlow>(this, pageSize);

        #endregion

        #region - IPullApiClient<IFlow, IPublishableFlow> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IPublishableFlow>> PullAsync(IFlow contentItem, CancellationToken cancel)
        {
            var downloadResult = await DownloadFlowAsync(contentItem.Id, cancel).ConfigureAwait(false);
            if (!downloadResult.Success)
            {
                return downloadResult.CastFailure<IPublishableFlow>();
            }

            await using (downloadResult)
            {
                var file = await _fileStore.CreateAsync(contentItem, downloadResult.Value, cancel).ConfigureAwait(false);

                var publishableFlow = new PublishableFlow(contentItem, file);
                return Result<IPublishableFlow>.Succeeded(publishableFlow);
            }
        }

        #endregion

        #region - IPublishApiClient<IPublishableFlow, IFlow> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IFlow>> PublishAsync(IPublishableFlow item, CancellationToken cancel)
        {
            var fileStream = await item.File.OpenReadAsync(cancel).ConfigureAwait(false);

            await using (fileStream)
            {
                var publishOptions = new PublishFlowOptions(item, fileStream.Content);
                var publishResult = await PublishFlowAsync(publishOptions, cancel)
                    .ConfigureAwait(false);

                return publishResult;
            }
        }

        #endregion
    }
}
