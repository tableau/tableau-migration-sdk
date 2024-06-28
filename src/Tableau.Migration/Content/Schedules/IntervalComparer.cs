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

namespace Tableau.Migration.Content.Schedules
{
    internal class IntervalComparer
        : ComparerBase<IInterval>
    {
        private static readonly IList<string?> AllWeekDays = [.. IntervalValues.WeekDaysValues];
        private static readonly IList<string?> AllMonthDays = [.. IntervalValues.MonthDaysValues];

        protected override int CompareItems(
            IInterval x,
            IInterval y) 
        {
            if (x is null && y is null)
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            var diff = (x.Minutes ?? 0) - (y.Minutes ?? 0);

            if (diff != 0)
            {
                return diff;
            }

            diff = (x.Hours ?? 0) - (y.Hours ?? 0);

            if (diff != 0)
            {
                return diff;
            }

            diff = AllWeekDays.IndexOf(x.WeekDay ?? string.Empty) - AllWeekDays.IndexOf(y.WeekDay ?? string.Empty);

            if (diff != 0)
            {
                return diff;
            }

            diff = AllMonthDays.IndexOf(x.WeekDay ?? string.Empty) - AllMonthDays.IndexOf(y.WeekDay ?? string.Empty);

            return diff;
        }
    }
}
