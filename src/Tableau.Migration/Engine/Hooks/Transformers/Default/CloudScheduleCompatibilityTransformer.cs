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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;


using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that changes extract refresh tasks to cloud supported ones.     
    /// </summary>       
    public class CloudScheduleCompatibilityTransformer<TWithSchedule>
        : ContentTransformerBase<TWithSchedule>
        where TWithSchedule : IWithSchedule<ICloudSchedule>
    {
        /// <summary>
        /// Creates a new <see cref="CloudScheduleCompatibilityTransformer{TWithSchedule}"/> object.
        /// </summary>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">The logger used to log messages.</param>
        public CloudScheduleCompatibilityTransformer(
            ISharedResourcesLocalizer localizer,
            ILogger<CloudScheduleCompatibilityTransformer<TWithSchedule>> logger)
            : base(localizer, logger)
        { }

        /// <inheritdoc />
        public override Task<TWithSchedule?> TransformAsync(
            TWithSchedule itemToTransform,
            CancellationToken cancel)
        {
            var currentFrequency = itemToTransform.Schedule.Frequency;
            var currentIntervals = itemToTransform.Schedule.FrequencyDetails.Intervals;

            if (currentFrequency.IsCloudCompatible(currentIntervals))
            {
                return Task.FromResult<TWithSchedule?>(itemToTransform);
            }

            var newIntervals = currentFrequency.ToCloudCompatible(currentIntervals);

            if (Logger.LogIntervalsChanges(
                Localizer[SharedResourceKeys.IntervalsChangedWarning],
                itemToTransform.Id, 
                currentIntervals, 
                newIntervals))
            {
                itemToTransform.Schedule.FrequencyDetails.Intervals = newIntervals;
            }

            return Task.FromResult<TWithSchedule?>(itemToTransform);
        }
    }
}
