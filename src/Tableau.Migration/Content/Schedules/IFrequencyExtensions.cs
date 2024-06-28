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

using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Schedules
{
    internal static class IFrequencyExtensions
    {
        private const int ALLOWED_SELECTED_WEEKDAYS_WEEKLY = 1;
        private const int ALLOWED_CLOUD_MINUTES = 60;

        public static IList<IInterval> ToCloudCompatible(
            this string? frequency,
            IList<IInterval> intervals)
        {
            if (frequency.IsCloudCompatible(intervals))
            {
                return intervals;
            }

            if (ScheduleFrequencies.IsAMatch(frequency, ScheduleFrequencies.Hourly))
            {
                return intervals
                    .Select(i => i.Minutes is not null && i.Minutes.Value != ALLOWED_CLOUD_MINUTES
                        ? Interval.WithHours(1)
                        : i)
                    .ToList();
            }

            return intervals
                .Where(i => i.WeekDay is not null)
                .Take(ALLOWED_SELECTED_WEEKDAYS_WEEKLY)
                .ToList();
        }

        public static bool IsCloudCompatible(
            this string? frequency,
            IList<IInterval> intervals)
        {
            if (ScheduleFrequencies.IsAMatch(frequency, ScheduleFrequencies.Hourly))
            {
                return intervals.All(i =>
                    i.Minutes is null ||
                    (
                        i.Minutes is not null &&
                        i.Minutes.Value == ALLOWED_CLOUD_MINUTES
                    ));
            }

            if (ScheduleFrequencies.IsAMatch(frequency, ScheduleFrequencies.Weekly))
            {
                return intervals.Where(i => i.WeekDay is not null).Count() == ALLOWED_SELECTED_WEEKDAYS_WEEKLY;
            }

            return true;
        }
    }
}
