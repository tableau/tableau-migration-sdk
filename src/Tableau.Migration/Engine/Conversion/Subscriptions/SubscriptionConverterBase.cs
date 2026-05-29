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

using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Engine.Conversion.Schedules;

namespace Tableau.Migration.Engine.Conversion.Subscriptions
{
    /// <summary>
    /// Base class for converting subscriptions from one type to another.
    /// </summary>
    internal abstract class SubscriptionConverterBase<TSourceSubscription, TSourceSchedule, TTargetSubscription, TTargetSchedule>
        : ScheduledTaskConverterBase<TSourceSubscription, TSourceSchedule, TTargetSubscription, TTargetSchedule>,
          ISubscriptionConverter<TSourceSubscription, TTargetSubscription>
        where TSourceSubscription : ISubscription<TSourceSchedule>
        where TSourceSchedule : ISchedule
        where TTargetSubscription : ISubscription<TTargetSchedule>
        where TTargetSchedule : ISchedule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionConverterBase{TSourceSubscription, TSourceSchedule, TTargetSubscription, TTargetSchedule}"/> class.
        /// </summary>
        /// <param name="scheduleConverter">The schedule converter to use for converting schedules.</param>
        protected SubscriptionConverterBase(IScheduleConverter<TSourceSchedule, TTargetSchedule> scheduleConverter)
            : base(scheduleConverter)
        { }

        /// <summary>
        /// Converts the subscription after the schedule has been converted.
        /// </summary>
        /// <param name="source">The source subscription.</param>
        /// <param name="targetSchedule">The converted target schedule.</param>
        /// <returns>The converted target subscription.</returns>
        protected abstract TTargetSubscription ConvertSubscription(TSourceSubscription source, TTargetSchedule targetSchedule);

        /// <inheritdoc />
        protected override TTargetSubscription ConvertTask(TSourceSubscription source, TTargetSchedule targetSchedule)
            => ConvertSubscription(source, targetSchedule);
    }
}
