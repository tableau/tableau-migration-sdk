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
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Schedules
{
    internal static class IntervalValues
    {
        public static readonly IImmutableList<int> ServerHoursValues = new int[] { 1, 2, 4, 6, 8, 12, 24 }
            .ToImmutableList();

        public static readonly IImmutableList<int> CloudHoursValues = new int[] { 2, 4, 6, 8, 12, 24 }
            .ToImmutableList();

        public static readonly IImmutableList<int> MinutesValues = new int[] { 15, 30, 60 }
            .ToImmutableList();

        public static readonly IImmutableList<string> WeekDaysValues = WeekDays.GetAll()
            .ToImmutableList();

        public static readonly IImmutableList<string> MonthDaysValues = Enumerable.Range(1, 31)
            .Select(d => d.ToString())
            .Append(OccurrenceOfWeekday.First.ToString())
            .Append(OccurrenceOfWeekday.Second.ToString())
            .Append(OccurrenceOfWeekday.Third.ToString())
            .Append(OccurrenceOfWeekday.Fourth.ToString())
            .Append(OccurrenceOfWeekday.Fifth.ToString())
            .Append(OccurrenceOfWeekday.LastDay.ToString())
            .ToImmutableList();

        public static readonly IImmutableList<string> MonthDayNumberValues = Enumerable.Range(1, 31)
            .Select(d => d.ToString())
            .Append("LastDay")
            .ToImmutableList();

        public static readonly IImmutableList<string> MonthDayOccurrenceValues = Enum.GetNames(typeof(OccurrenceOfWeekday))
            .ToImmutableList();

        internal enum OccurrenceOfWeekday
        {
            First,
            Second,
            Third,
            Fourth,
            Fifth,
            LastDay
        }
    }
}
