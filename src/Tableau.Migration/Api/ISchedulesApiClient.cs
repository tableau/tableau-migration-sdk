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
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client job operations.
    /// </summary>
    public interface ISchedulesApiClient :
        IContentApiClient,
        IReadApiClient<IServerSchedule>
    {
        /// <summary>
        /// Gets the paged list of extract refresh tasks for a given schedule ID.
        /// </summary>
        /// <param name="scheduleId">The schedule's ID.</param>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The extract refresh tasks for the given schedule ID.</returns>
        Task<IPagedResult<IScheduleExtractRefreshTask>> GetScheduleExtractRefreshTasksAsync(
            Guid scheduleId,
            int pageNumber,
            int pageSize,
            CancellationToken cancel);

        /// <summary>
        /// Gets all the extract refresh tasks for a given schedule ID.
        /// </summary>
        /// <param name="scheduleId">The schedule's ID.</param>      
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The extract refresh tasks for the given schedule ID.</returns>
        Task<IResult<IImmutableList<IScheduleExtractRefreshTask>>> GetAllScheduleExtractRefreshTasksAsync(
            Guid scheduleId,
            CancellationToken cancel);
    }
}
