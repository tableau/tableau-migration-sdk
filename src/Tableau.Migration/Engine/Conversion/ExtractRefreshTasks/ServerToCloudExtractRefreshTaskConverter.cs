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

using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Conversion.ExtractRefreshTasks;
using Tableau.Migration.Engine.Conversion.Schedules;

namespace Tableau.Migration.ContentConverters.Schedules
{
    /// <summary>
    /// Converter for converting ServerExtractRefreshTask to CloudExtractRefreshTask.
    /// </summary>
    internal class ServerToCloudExtractRefreshTaskConverter :
        ExtractRefreshTaskConverterBase<IServerExtractRefreshTask, IServerSchedule, ICloudExtractRefreshTask, ICloudSchedule>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerToCloudExtractRefreshTaskConverter"/> class.
        /// </summary>
        /// <param name="scheduleConverter">The schedule converter.</param>
        public ServerToCloudExtractRefreshTaskConverter(IScheduleConverter<IServerSchedule, ICloudSchedule> scheduleConverter)
            : base(scheduleConverter)
        { }

        /// <summary>
        /// Creates a new instance of the target extract refresh task.
        /// </summary>
        /// <param name="source">The source extract refresh task.</param>
        /// <param name="targetSchedule">The converted target schedule.</param>
        /// <returns>A new instance of the target extract refresh task.</returns>
        protected override ICloudExtractRefreshTask ConvertExtractRefreshTask(IServerExtractRefreshTask source, ICloudSchedule targetSchedule)
        {
            var type = source.Type;
            if (type == ExtractRefreshType.ServerIncrementalRefresh)
            {
                type = ExtractRefreshType.CloudIncrementalRefresh;
            }

            return new CloudExtractRefreshTask(source.Id, type, source.ContentType, source.Content, targetSchedule);
        }
    }
}
