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

using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Engine.Conversion.Schedules;

namespace Tableau.Migration.Engine.Conversion.ExtractRefreshTasks
{
    /// <summary>
    /// Base class for converting extract refresh tasks from one type to another.
    /// </summary>
    internal abstract class ExtractRefreshTaskConverterBase<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>
        : ScheduledTaskConverterBase<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>,
          IExtractRefreshTaskConverter<TSourceTask, TTargetTask>
        where TSourceTask : IExtractRefreshTask<TSourceSchedule>
        where TSourceSchedule : ISchedule
        where TTargetTask : IExtractRefreshTask<TTargetSchedule>
        where TTargetSchedule : ISchedule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractRefreshTaskConverterBase{TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule}"/> class.
        /// </summary>
        /// <param name="scheduleConverter">The schedule converter to use for converting schedules.</param>
        protected ExtractRefreshTaskConverterBase(IScheduleConverter<TSourceSchedule, TTargetSchedule> scheduleConverter)
            : base(scheduleConverter)
        { }

        /// <summary>
        /// Converts the extract refresh task after the schedule has been converted.
        /// </summary>
        /// <param name="source">The source extract refresh task.</param>
        /// <param name="targetSchedule">The converted target schedule.</param>
        /// <returns>The converted target extract refresh task.</returns>
        protected abstract TTargetTask ConvertExtractRefreshTask(TSourceTask source, TTargetSchedule targetSchedule);

        /// <inheritdoc />
        protected override TTargetTask ConvertTask(TSourceTask source, TTargetSchedule targetSchedule)
            => ConvertExtractRefreshTask(source, targetSchedule);
    }
}
