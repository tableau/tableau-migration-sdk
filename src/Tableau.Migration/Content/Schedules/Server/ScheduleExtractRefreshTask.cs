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
using Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Content.Schedules.Server
{
    /// <summary>
    /// The class for an extract.
    /// </summary>
    public class ScheduleExtractRefreshTask : IScheduleExtractRefreshTask
    {
        /// <summary>
        /// Constructor to build from a <see cref="ScheduleExtractRefreshTasksResponse.ExtractType"/>.
        /// </summary>
        /// <param name="response"></param>
        public ScheduleExtractRefreshTask(ScheduleExtractRefreshTasksResponse.ExtractType response)
        {
            Guard.AgainstNull(response, nameof(response));
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Priority = response.Priority;
            Type = Guard.AgainstNull(response.Type, () => response.Type);
            WorkbookId = response.Workbook?.Id;
            DatasourceId = response.DataSource?.Id;
        }

        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public int Priority { get; set; }

        /// <inheritdoc/>
        public string Type { get; set; }

        /// <inheritdoc/>
        public Guid? WorkbookId { get; set; }

        /// <inheritdoc/>
        public Guid? DatasourceId { get; set; }

    }
}
