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
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API schedule methods.
    /// </summary>
    public sealed class SchedulesRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated schedule query API method.
        /// </summary>
        public MethodSimulator GetServerSchedule { get; }

        /// <summary>
        /// Gets the simulated schedules extract refresh tasks query API method.
        /// </summary>
        public MethodSimulator GetExtractRefreshTasks { get; }

        /// <summary>
        /// Creates a new <see cref="SchedulesRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public SchedulesRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            GetServerSchedule = simulator.SetupRestGetById<ScheduleResponse, ScheduleResponse.ScheduleType>(
                EntityUrl("schedules"), d => d.Schedules);
            GetExtractRefreshTasks = simulator.SetupRestPagedList<ScheduleExtractRefreshTasksResponse, ScheduleExtractRefreshTasksResponse.ExtractType>(
                SiteEntityUrl("schedules", "extracts"), (d, r) =>
                {
                    var scheduleId = r.GetIdAfterSegment("schedules");

                    if (scheduleId is null || !d.ScheduleExtracts.ContainsKey(scheduleId.Value))
                    {
                        return Array.Empty<ScheduleExtractRefreshTasksResponse.ExtractType>().ToList();
                    }

                    return d.ScheduleExtractRefreshTasks
                        .Where(t => d.ScheduleExtracts[scheduleId.Value].Contains(t.Id))
                        .ToList();
                });
        }
    }
}
