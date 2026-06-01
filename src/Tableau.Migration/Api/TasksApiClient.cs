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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models.Cloud;
using Tableau.Migration.Api.Paging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests.Cloud;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

using CloudRequests = Tableau.Migration.Api.Rest.Models.Requests.Cloud;
using CloudResponses = Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using ServerResponses = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client tasks operations.
    /// </summary>
    internal class TasksApiClient : ContentApiClientBase, ITasksApiClient
    {
        private readonly IServerSessionProvider _sessionProvider;
        private readonly IContentCacheFactory _contentCacheFactory;
        private readonly IHttpContentSerializer _serializer;

        public TasksApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            IContentCacheFactory contentCacheFactory,
            IServerSessionProvider sessionProvider,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IHttpContentSerializer serializer)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer, RestUrlKeywords.Tasks)
        {
            _sessionProvider = sessionProvider;
            _contentCacheFactory = contentCacheFactory;
            _serializer = serializer;
        }

        #region - ITasksApiClient -

        /// <inheritdoc />
        public IServerTasksApiClient ForServer()
            => ExecuteForInstanceType(TableauInstanceType.Server, _sessionProvider.InstanceType, () => this);

        /// <inheritdoc />
        public ICloudTasksApiClient ForCloud()
            => ExecuteForInstanceType(TableauInstanceType.Cloud, _sessionProvider.InstanceType, () => this);

        #endregion

        #region - ICloudTasksApiClient -

        /// <inheritdoc/>
        public async Task<IResult> DeleteExtractRefreshTaskAsync(
            Guid extractRefreshTaskId,
            CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{RestUrlKeywords.ExtractRefreshes}/{extractRefreshTaskId.ToUrlSegment()}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        async Task<IResult<IImmutableList<ICloudExtractRefreshTask>>> ICloudTasksApiClient.GetAllExtractRefreshTasksAsync(
            CancellationToken cancel)
            => await GetAllExtractRefreshTasksAsync<CloudResponses.ExtractRefreshTasksResponse, ICloudExtractRefreshTask, ICloudSchedule>(
                (r, c) => CloudExtractRefreshTask.CreateManyAsync(r, ContentFinderFactory, Logger, SharedResourcesLocalizer, c),
                cancel)
                .ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IResult<ICloudExtractRefreshTask>> ICloudTasksApiClient.CreateExtractRefreshTaskAsync(
            ICreateExtractRefreshTaskOptions options,
            CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{RestUrlKeywords.ExtractRefreshes}")
                .ForPostRequest()
                .WithXmlContent(new CreateExtractRefreshTaskRequest(options))
                .SendAsync<CloudResponses.CreateExtractRefreshTaskResponse>(cancel)
                .ToResultAsync(async (r, c) =>
                {
                    var task = Guard.AgainstNull(r.Item, () => r.Item);
                    var finder = ContentFinderFactory.ForExtractRefreshContent(task.GetContentType());

                    var contentReference = await finder.FindByIdAsync(task.GetContentId(), cancel).ConfigureAwait(false);

                    // Since we published with a content reference, we expect the reference returned is valid/knowable.
                    Guard.AgainstNull(contentReference, () => contentReference);

                    return CloudExtractRefreshTask.Create(task, r.Schedule, contentReference);
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<IResult<ICloudExtractRefreshTask>> PublishAsync(
            ICloudExtractRefreshTask item,
            CancellationToken cancel)
        {
            var options = new CreateExtractRefreshTaskOptions(
                item.Type,
                item.ContentType,
                item.Content.Id,
                item.Schedule);

            return await ForCloud()
                .CreateExtractRefreshTaskAsync(options, cancel)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        async Task<IResult<IImmutableList<ICloudFlowRunTask>>> ICloudTasksApiClient.GetAllFlowRunTasksAsync(CancellationToken cancel)
        {
            return await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/runFlow")
                .ForGetRequest()
                .SendAsync<ServerResponses.FlowRunTasksResponse>(cancel)
                .ToResultAsync(
                    async (r, c) => await CloudFlowRunTask.CreateManyAsync(
                        r,
                        ContentFinderFactory,
                        Logger,
                        c)
                        .ConfigureAwait(false),
                    SharedResourcesLocalizer,
                    cancel)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        async Task<IResult<ICloudFlowRunTask>> ICloudTasksApiClient.CreateCloudFlowTaskAsync(
            ICreateCloudFlowTaskOptions options,
            CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{RestUrlKeywords.Flows}")
                .ForPostRequest()
                .WithXmlContent(new CloudRequests.CreateCloudFlowTaskRequest(options))
                .SendAsync<CloudResponses.CreateCloudFlowTaskResponse>(cancel)
                .ToResultAsync(async (r, c) =>
                {
                    var item = Guard.AgainstNull(r.Item, () => r.Item);
                    var task = Guard.AgainstNull(item.FlowRun, () => item.FlowRun);
                    var flow = Guard.AgainstNull(task.Flow, () => task.Flow);
                    var schedule = Guard.AgainstNull(task.Schedule, () => task.Schedule);

                    var finder = ContentFinderFactory.ForContentType<IFlow>();
                    var flowReference = await finder.FindByIdAsync(flow.Id, cancel).ConfigureAwait(false);

                    // Since we published with a flow reference, we expect the reference returned is valid/knowable.
                    Guard.AgainstNull(flowReference, () => flowReference);

                    return CloudFlowRunTask.Create(task, schedule, flowReference);
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<IResult<ICloudFlowRunTask>> PublishAsync(
            ICloudFlowRunTask item,
            CancellationToken cancel)
        {
            var options = new CreateCloudFlowTaskOptions(
                item.Flow.Id,
                item.Schedule,
                flowParameterSpecs: null,
                flowOutputStepIds: null);

            return await ForCloud()
                .CreateCloudFlowTaskAsync(options, cancel)
                .ConfigureAwait(false);
        }

        #endregion

        #region - IServerTasksApiClient -

        /// <inheritdoc />
        async Task<IResult<IImmutableList<IServerExtractRefreshTask>>> IServerTasksApiClient.GetAllExtractRefreshTasksAsync(CancellationToken cancel)
            => await GetAllExtractRefreshTasksAsync<ServerResponses.ExtractRefreshTasksResponse, IServerExtractRefreshTask, IServerSchedule>(
                (r, c) => ServerExtractRefreshTask.CreateManyAsync(r, ContentFinderFactory, _contentCacheFactory, Logger, SharedResourcesLocalizer, c),
                cancel)
                .ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IResult<IImmutableList<IScheduleFlowRunTask>>> IServerTasksApiClient.GetAllFlowRunTasksAsync(CancellationToken cancel)
        {
            return await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/runFlow")
                .ForGetRequest()
                .SendAsync<ServerResponses.FlowRunTasksResponse>(cancel)
                .ToResultAsync(
                    (r, c) => Task.FromResult<IImmutableList<IScheduleFlowRunTask>>(
                        r.Items
                            .Where(t => t.FlowRun != null)
                            .Select(t => new ScheduleFlowRunTask(t.FlowRun!))
                            .Cast<IScheduleFlowRunTask>()
                            .ToImmutableList()),
                    SharedResourcesLocalizer,
                    cancel)
                .ConfigureAwait(false);
        }

        #endregion

        #region - IPagedListApiClient<IServerExtractRefreshTask> Implementation -

        /// <inheritdoc />
        public IPager<IServerExtractRefreshTask> GetPager(int pageSize)
            => new ApiListPager<IServerExtractRefreshTask>(this, pageSize);

        #endregion

        #region - IApiPageAccessor<IServerExtractRefreshTask> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IServerExtractRefreshTask>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            if (pageNumber != 1)
            {
                return PagedResult<IServerExtractRefreshTask>.Succeeded(
                    ImmutableArray<IServerExtractRefreshTask>.Empty,
                    pageNumber,
                    pageSize,
                    0,
                    true);
            }

            var loadResult = await ForServer().GetAllExtractRefreshTasksAsync(cancel).ConfigureAwait(false);

            if (!loadResult.Success)
            {
                return PagedResult<IServerExtractRefreshTask>.Failed(loadResult.Errors);
            }

            return PagedResult<IServerExtractRefreshTask>.Succeeded(
                loadResult.Value!,
                pageNumber,
                loadResult.Value.Count,
                loadResult.Value.Count,
                true);
        }

        #endregion

        #region - IPagedListApiClient<IServerFlowRunTask> Implementation -

        /// <inheritdoc />
        IPager<IServerFlowRunTask> IPagedListApiClient<IServerFlowRunTask>.GetPager(int pageSize)
            => new ApiListPager<IServerFlowRunTask>(this, pageSize);

        #endregion

        #region - IApiPageAccessor<IServerFlowRunTask> Implementation -

        /// <inheritdoc />
        async Task<IPagedResult<IServerFlowRunTask>> IApiPageAccessor<IServerFlowRunTask>.GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            if (pageNumber != 1)
            {
                return PagedResult<IServerFlowRunTask>.Succeeded(
                    ImmutableArray<IServerFlowRunTask>.Empty,
                    pageNumber,
                    pageSize,
                    0,
                    true);
            }

            var loadResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/runFlow")
                .ForGetRequest()
                .SendAsync<ServerResponses.FlowRunTasksResponse>(cancel)
                .ToResultAsync(
                    async (r, c) => await ServerFlowRunTask.CreateManyAsync(
                        r,
                        ContentFinderFactory,
                        _contentCacheFactory,
                        Logger,
                        c)
                        .ConfigureAwait(false),
                    SharedResourcesLocalizer,
                    cancel)
                .ConfigureAwait(false);

            if (!loadResult.Success)
            {
                return PagedResult<IServerFlowRunTask>.Failed(loadResult.Errors);
            }

            return PagedResult<IServerFlowRunTask>.Succeeded(
                loadResult.Value!,
                pageNumber,
                loadResult.Value.Count,
                loadResult.Value.Count,
                true);
        }

        #endregion

        #region - IPagedListApiClient<ICloudFlowRunTask> Implementation -

        /// <inheritdoc />
        IPager<ICloudFlowRunTask> IPagedListApiClient<ICloudFlowRunTask>.GetPager(int pageSize)
            => new ApiListPager<ICloudFlowRunTask>(this, pageSize);

        #endregion

        #region - IApiPageAccessor<ICloudFlowRunTask> Implementation -

        /// <inheritdoc />
        async Task<IPagedResult<ICloudFlowRunTask>> IApiPageAccessor<ICloudFlowRunTask>.GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            if (pageNumber != 1)
            {
                return PagedResult<ICloudFlowRunTask>.Succeeded(
                    ImmutableArray<ICloudFlowRunTask>.Empty,
                    pageNumber,
                    pageSize,
                    0,
                    true);
            }

            var loadResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/runFlow")
                .ForGetRequest()
                .SendAsync<ServerResponses.FlowRunTasksResponse>(cancel)
                .ToResultAsync(
                    async (r, c) => await CloudFlowRunTask.CreateManyAsync(
                        r,
                        ContentFinderFactory,
                        Logger,
                        c)
                        .ConfigureAwait(false),
                    SharedResourcesLocalizer,
                    cancel)
                .ConfigureAwait(false);

            if (!loadResult.Success)
            {
                return PagedResult<ICloudFlowRunTask>.Failed(loadResult.Errors);
            }

            return PagedResult<ICloudFlowRunTask>.Succeeded(
                loadResult.Value!,
                pageNumber,
                loadResult.Value.Count,
                loadResult.Value.Count,
                true);
        }

        #endregion

        #region - IReadApiClient<IServerExtractRefreshTask> Implementation -

        /// <inheritdoc />
        async Task<IResult<IServerExtractRefreshTask>> IReadApiClient<IServerExtractRefreshTask>.GetByIdAsync(Guid contentId, CancellationToken cancel)
        {
            AssertInstanceType(TableauInstanceType.Server, _sessionProvider.InstanceType, throwOnFailure: true);

            return await GetExtractRefreshTaskAsync<ServerResponses.ExtractRefreshTaskResponse, IServerExtractRefreshTask, IServerSchedule>(
                contentId,
                (r, c) => ServerExtractRefreshTask.CreateAsync(r, ContentFinderFactory, _contentCacheFactory, Logger, SharedResourcesLocalizer, c),
                cancel).ConfigureAwait(false);
        }

        #endregion

        #region - IReadApiClient<ICloudExtractRefreshTask> Implementation -

        /// <inheritdoc />
        async Task<IResult<ICloudExtractRefreshTask>> IReadApiClient<ICloudExtractRefreshTask>.GetByIdAsync(Guid contentId, CancellationToken cancel)
        {
            AssertInstanceType(TableauInstanceType.Cloud, _sessionProvider.InstanceType, throwOnFailure: true);

            return await GetExtractRefreshTaskAsync<CloudResponses.ExtractRefreshTaskResponse, ICloudExtractRefreshTask, ICloudSchedule>(
                contentId,
                (r, c) => CloudExtractRefreshTask.CreateAsync(r, ContentFinderFactory, Logger, SharedResourcesLocalizer, c),
                cancel).ConfigureAwait(false);
        }

        #endregion

        private async Task<IResult<IImmutableList<TExtractRefreshTask>>> GetAllExtractRefreshTasksAsync<TResponse, TExtractRefreshTask, TSchedule>(
            Func<TResponse, CancellationToken, Task<IImmutableList<TExtractRefreshTask>>> responseItemFactory,
            CancellationToken cancel)
            where TResponse : TableauServerResponse
            where TExtractRefreshTask : IExtractRefreshTask<TSchedule>
            where TSchedule : ISchedule
        {
            return await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{RestUrlKeywords.ExtractRefreshes}")
                .ForGetRequest()
                .SendAsync<TResponse>(cancel)
                .ToResultAsync(
                    (r, c) => responseItemFactory(r, c),
                    SharedResourcesLocalizer,
                    cancel)
                .ConfigureAwait(false);
        }

        private async Task<IResult<TExtractRefreshTask>> GetExtractRefreshTaskAsync<TResponse, TExtractRefreshTask, TSchedule>(Guid id,
            Func<TResponse, CancellationToken, Task<TExtractRefreshTask>> responseItemFactory,
            CancellationToken cancel)
            where TResponse : TableauServerResponse
            where TExtractRefreshTask : class, IExtractRefreshTask<TSchedule>
            where TSchedule : ISchedule
        {
            return await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{RestUrlKeywords.ExtractRefreshes}/{id.ToUrlSegment()}")
                .ForGetRequest()
                .SendAsync<TResponse>(cancel)
                .ToResultAsync(
                    (r, c) => responseItemFactory(r, c),
                    SharedResourcesLocalizer,
                    cancel)
                .ConfigureAwait(false);
        }
    }
}
