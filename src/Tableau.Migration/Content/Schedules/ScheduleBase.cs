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

using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content.Schedules
{
    internal abstract class ScheduleBase : ISchedule
    {
        /// <inheritdoc />
        public string Frequency { get; set; }

        /// <inheritdoc />
        public IFrequencyDetails FrequencyDetails { get; }

        /// <inheritdoc />
        public string? NextRunAt { get; }
        
        /// <summary>
        /// Creates a new <see cref="ScheduleBase"/> instance.
        /// </summary>
        /// <param name="response">The REST API schedule response.</param>
        public ScheduleBase(IScheduleType response)
        {
            Frequency = Guard.AgainstNullEmptyOrWhiteSpace(response.Frequency, () => response.Frequency);

            NextRunAt = response.NextRunAt;

            var frequencyDetails = Guard.AgainstNull(response.FrequencyDetails, () => response.FrequencyDetails);

            FrequencyDetails = new FrequencyDetails(frequencyDetails);
        }

        /// <summary>
        /// Creates a new <see cref="ScheduleBase"/> instance.
        /// </summary>
        public ScheduleBase(string frequency, IFrequencyDetails frequencyDetails, string? nextRunAt)
        {
            Frequency = Guard.AgainstNullEmptyOrWhiteSpace(frequency, nameof(frequency));
            NextRunAt = nextRunAt;
            FrequencyDetails = frequencyDetails;
        }
    }
}
