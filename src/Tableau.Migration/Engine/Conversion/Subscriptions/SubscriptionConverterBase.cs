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
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Engine.Conversion.Schedules;

namespace Tableau.Migration.Engine.Conversion.Subscriptions
{
    internal abstract class SubscriptionConverterBase<TSourceSubscription, TSourceSchedule, TTargetSubscription, TTargetSchedule> : ISubscriptionConverter<TSourceSubscription, TTargetSubscription>
        where TSourceSubscription : ISubscription<TSourceSchedule>
        where TSourceSchedule : ISchedule
        where TTargetSubscription : ISubscription<TTargetSchedule>
        where TTargetSchedule : ISchedule
    {
        private readonly IScheduleConverter<TSourceSchedule, TTargetSchedule> _scheduleConverter;

        protected SubscriptionConverterBase(IScheduleConverter<TSourceSchedule, TTargetSchedule> scheduleConverter)
        {
            _scheduleConverter = scheduleConverter;
        }

        /// <inheritdoc />
        public async Task<TTargetSubscription> ConvertAsync(TSourceSubscription source, CancellationToken cancel)
        {
            var targetSchedule = await _scheduleConverter.ConvertAsync(source.Schedule, cancel).ConfigureAwait(false);
            return ConvertSubscription(source, targetSchedule);
        }

        protected abstract TTargetSubscription ConvertSubscription(TSourceSubscription source, TTargetSchedule targetSchedule);
    }
}
