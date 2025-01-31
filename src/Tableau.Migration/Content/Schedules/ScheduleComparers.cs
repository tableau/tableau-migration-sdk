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

using System;

namespace Tableau.Migration.Content.Schedules
{
    internal class ScheduleComparers : ComparerBase<ISchedule>
    {
        public static readonly IntervalComparer IntervalsComparer = new();

        protected override int CompareItems(ISchedule? x, ISchedule? y)
        {
            if (x is null && y is null)
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            var frequencyComparison = string.Compare(x.Frequency, y.Frequency, StringComparison.Ordinal);
            if (frequencyComparison != 0)
                return frequencyComparison;

            var startAtComparison = Nullable.Compare(x.FrequencyDetails.StartAt, y.FrequencyDetails.StartAt);
            if (startAtComparison != 0)
                return startAtComparison;

            var endAtComparison = Nullable.Compare(x.FrequencyDetails.EndAt, y.FrequencyDetails.EndAt);
            if (endAtComparison != 0)
                return endAtComparison;

            var intervalsComparison = IntervalsComparer.Compare(x.FrequencyDetails.Intervals, y.FrequencyDetails.Intervals);
            if (intervalsComparison != 0)
                return intervalsComparison;

            return string.Compare(x.NextRunAt, y.NextRunAt, StringComparison.Ordinal);
        }
    }
}
