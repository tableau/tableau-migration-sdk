//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing extract refresh tasks data for migration tests.
    /// </summary>
    public static class ExtractRefreshTasksDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <returns>The list of prepared extract refresh tasks.</returns>
        public static List<ExtractRefreshTasksResponse.TaskType> PrepareServerSource(TableauApiSimulator sourceApi)
        {
            var extractRefreshTasks = new List<ExtractRefreshTasksResponse.TaskType>();

            var schedules = SchedulesDataPreparation.PrepareServerSource(sourceApi);

            var count = 0;
            foreach (var datasource in sourceApi.Data.DataSources)
            {
                extractRefreshTasks.Add(CreateDataSourceExtractRefreshTask(schedules, count, datasource.Id));
                count++;
            }

            foreach (var workbook in sourceApi.Data.Workbooks)
            {
                extractRefreshTasks.Add(CreateWorkbookExtractRefreshTask(schedules, count, workbook.Id));
                count++;
            }

            foreach (var extractRefreshTask in extractRefreshTasks)
            {
                var schedule = schedules.First(sch => sch.Id == extractRefreshTask.ExtractRefresh!.Schedule!.Id);
                sourceApi.Data.ServerExtractRefreshTasks.Add(extractRefreshTask);
                sourceApi.Data.AddExtractToSchedule(
                    new ScheduleExtractRefreshTasksResponse.ExtractType
                    {
                        Id = extractRefreshTask.ExtractRefresh!.Id,
                        Type = count % 2 == 0 ? ExtractRefreshType.FullRefresh : ExtractRefreshType.ServerIncrementalRefresh
                    },
                    schedule);
                count++;
            }

            return extractRefreshTasks;
        }

        private static ExtractRefreshTasksResponse.TaskType CreateWorkbookExtractRefreshTask(List<ScheduleResponse.ScheduleType> schedules, int count, Guid workbookId)
            => new()
            {
                ExtractRefresh = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType
                {
                    Id = Guid.NewGuid(),
                    Priority = 50,
                    Workbook = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.WorkbookType
                    {
                        Id = workbookId
                    },
                    Schedule = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType
                    {
                        Id = GetScheduleId(schedules, count)
                    }
                }
            };

        private static ExtractRefreshTasksResponse.TaskType CreateDataSourceExtractRefreshTask(List<ScheduleResponse.ScheduleType> schedules, int count, Guid dataSourceId)
            => new()
            {
                ExtractRefresh = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType
                {
                    Id = Guid.NewGuid(),
                    Priority = 50,
                    DataSource = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.DataSourceType
                    {
                        Id = dataSourceId
                    },
                    Schedule = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType
                    {
                        Id = GetScheduleId(schedules, count)
                    }
                }
            };

        private static Guid GetScheduleId(List<ScheduleResponse.ScheduleType> schedules, int count)
            => schedules[count % schedules.Count].Id;
    }
}