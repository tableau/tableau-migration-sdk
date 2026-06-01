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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Engine.Conversion.Schedules;

namespace Tableau.Migration.Engine.Conversion
{
    /// <summary>
    /// Base class for converting scheduled tasks from one type to another.
    /// Handles schedule conversion and delegates task-specific conversion to derived classes.
    /// </summary>
    /// <typeparam name="TSourceTask">The source task type that implements IWithSchedule.</typeparam>
    /// <typeparam name="TSourceSchedule">The source schedule type.</typeparam>
    /// <typeparam name="TTargetTask">The target task type that implements IWithSchedule.</typeparam>
    /// <typeparam name="TTargetSchedule">The target schedule type.</typeparam>
    internal abstract class ScheduledTaskConverterBase<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>
        : IContentItemConverter<TSourceTask, TTargetTask>
        where TSourceTask : IWithSchedule<TSourceSchedule>
        where TSourceSchedule : ISchedule
        where TTargetTask : IWithSchedule<TTargetSchedule>
        where TTargetSchedule : ISchedule
    {
        private readonly IScheduleConverter<TSourceSchedule, TTargetSchedule> _scheduleConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskConverterBase{TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule}"/> class.
        /// </summary>
        /// <param name="scheduleConverter">The schedule converter to use for converting schedules.</param>
        protected ScheduledTaskConverterBase(IScheduleConverter<TSourceSchedule, TTargetSchedule> scheduleConverter)
        {
            _scheduleConverter = scheduleConverter;
        }

        /// <inheritdoc />
        public async Task<TTargetTask> ConvertAsync(TSourceTask source, CancellationToken cancel)
        {
            var targetSchedule = await _scheduleConverter.ConvertAsync(source.Schedule, cancel).ConfigureAwait(false);
            return ConvertTask(source, targetSchedule);
        }

        /// <summary>
        /// Converts the task after the schedule has been converted.
        /// </summary>
        /// <param name="source">The source task.</param>
        /// <param name="targetSchedule">The converted target schedule.</param>
        /// <returns>The converted target task.</returns>
        protected abstract TTargetTask ConvertTask(TSourceTask source, TTargetSchedule targetSchedule);
    }
}
