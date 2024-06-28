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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal class SchedulesApiClient : ContentApiClientBase, ISchedulesApiClient
    {
        private readonly IContentCacheFactory _contentCacheFactory;
        private readonly IConfigReader _configReader;

        public SchedulesApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            IContentCacheFactory contentCacheFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IConfigReader configReader)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _contentCacheFactory = contentCacheFactory;
            _configReader = configReader;
        }

        #region - IReadApiClient<IServerSchedule> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IServerSchedule>> GetByIdAsync(
            Guid contentId,
            CancellationToken cancel)
        {
            var scheduleResult = await RestRequestBuilderFactory
                .CreateUri($"/schedules/{contentId.ToUrlSegment()}")
                .WithSiteId(null)
                .ForGetRequest()
                .SendAsync<ScheduleResponse>(cancel)
                .ToResultAsync(ServerSchedule.FromServerResponse, SharedResourcesLocalizer)
                .ConfigureAwait(false);

            if (!scheduleResult.Success)
            {
                return scheduleResult;
            }

            var serverSchedule = scheduleResult.Value;

            var refreshTasks = await GetAllScheduleExtractRefreshTasksAsync(contentId, cancel).ConfigureAwait(false);
            if (refreshTasks.Value != null)
            {
                serverSchedule.ExtractRefreshTasks.AddRange([.. refreshTasks.Value]);
            }

            var cache = _contentCacheFactory.ForContentType<IServerSchedule>(true);
            cache.AddOrUpdate(serverSchedule);

            return scheduleResult;
        }

        #endregion

        /// <inheritdoc />
        public async Task<IPagedResult<IScheduleExtractRefreshTask>> GetScheduleExtractRefreshTasksAsync(
            Guid scheduleId,
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
        {
            var extractsResult = await RestRequestBuilderFactory
                .CreateUri($"/schedules/{scheduleId.ToUrlSegment()}/extracts")
                .WithPage(pageNumber, pageSize)
                .ForGetRequest()
                .SendAsync<ScheduleExtractRefreshTasksResponse>(cancel)
                .ToPagedResultAsync(
                (response) =>
                    (new ScheduleExtractRefreshTasks(scheduleId, response)).ExtractRefreshTasks.ToImmutableList(),
                SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return extractsResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IImmutableList<IScheduleExtractRefreshTask>>> GetAllScheduleExtractRefreshTasksAsync(
            Guid scheduleId,
            CancellationToken cancel)
        {
            var configReader = _configReader;
            var pageSize = configReader.Get<IServerExtractRefreshTask>().BatchSize;

            IPager<IScheduleExtractRefreshTask> pager = new ScheduleExtractRefreshTasksResponsePager(
                this,
                scheduleId,
                pageSize);

            var refreshTasks = await pager.GetAllPagesAsync(cancel).ConfigureAwait(false);
            return refreshTasks;
        }
    }
}
