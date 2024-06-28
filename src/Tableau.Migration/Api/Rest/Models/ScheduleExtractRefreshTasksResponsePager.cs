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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Rest.Models
{
    internal sealed class ScheduleExtractRefreshTasksResponsePager
        : IndexedPagerBase<IScheduleExtractRefreshTask>, IPager<IScheduleExtractRefreshTask>
    {
        private readonly ISchedulesApiClient _apiClient;
        private readonly Guid _scheduleId;

        public ScheduleExtractRefreshTasksResponsePager(
            ISchedulesApiClient apiClient,
            Guid scheduleId,
            int pageSize)
            : base(pageSize)
        {
            _apiClient = apiClient;
            _scheduleId = scheduleId;
        }
        protected override async Task<IPagedResult<IScheduleExtractRefreshTask>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
        => await _apiClient.GetScheduleExtractRefreshTasksAsync(_scheduleId, pageNumber, pageSize, cancel).ConfigureAwait(false);
    }
}
