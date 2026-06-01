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

using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Engine.Conversion.Schedules;

namespace Tableau.Migration.Engine.Conversion.FlowRunTasks
{
    /// <summary>
    /// Base class for converting flow run tasks from one type to another.
    /// </summary>
    internal abstract class FlowRunTaskConverterBase<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>
        : ScheduledTaskConverterBase<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>,
          IFlowRunTaskConverter<TSourceTask, TTargetTask>
        where TSourceTask : IFlowRunTask<TSourceSchedule>
        where TSourceSchedule : ISchedule
        where TTargetTask : IFlowRunTask<TTargetSchedule>
        where TTargetSchedule : ISchedule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowRunTaskConverterBase{TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule}"/> class.
        /// </summary>
        /// <param name="scheduleConverter">The schedule converter to use for converting schedules.</param>
        protected FlowRunTaskConverterBase(IScheduleConverter<TSourceSchedule, TTargetSchedule> scheduleConverter)
            : base(scheduleConverter)
        { }

        /// <summary>
        /// Converts the flow run task after the schedule has been converted.
        /// </summary>
        /// <param name="source">The source flow run task.</param>
        /// <param name="targetSchedule">The converted target schedule.</param>
        /// <returns>The converted target flow run task.</returns>
        protected abstract TTargetTask ConvertFlowRunTask(TSourceTask source, TTargetSchedule targetSchedule);

        /// <inheritdoc />
        protected override TTargetTask ConvertTask(TSourceTask source, TTargetSchedule targetSchedule)
            => ConvertFlowRunTask(source, targetSchedule);
    }
}
