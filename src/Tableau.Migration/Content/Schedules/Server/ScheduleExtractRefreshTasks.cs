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
using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Content.Schedules.Server
{
    /// <summary>
    /// Class for a collection of extracts.
    /// </summary>
    public class ScheduleExtractRefreshTasks : IScheduleExtractRefreshTasks
    {
        /// <summary>
        /// Constructor to build from <see cref="ScheduleExtractRefreshTasksResponse"/>.
        /// </summary>
        /// <param name="scheduleId">The schedule ID.</param>
        /// <param name="response"></param>
        public ScheduleExtractRefreshTasks(Guid scheduleId, ScheduleExtractRefreshTasksResponse response)
        {
            Id = scheduleId;
            foreach (var item in response.Items)
            {
                ExtractRefreshTasks.Add(new ScheduleExtractRefreshTask(item));
            }
        }

        /// <inheritdoc/>
        public List<IScheduleExtractRefreshTask> ExtractRefreshTasks { get; set; } = [];

        /// <inheritdoc/>
        public Guid Id { get; }
    }
}
