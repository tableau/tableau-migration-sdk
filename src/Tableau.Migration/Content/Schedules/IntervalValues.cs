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

using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Schedules
{
    internal static class IntervalValues
    {
        public static readonly IImmutableList<int?> HoursValues = new int?[] { 1, 2, 4, 6, 8, 12, 24 }
            .Prepend(null)
            .ToImmutableList();

        public static readonly IImmutableList<int?> MinutesValues = new int?[] { 15, 30, 60 }
            .Prepend(null)
            .ToImmutableList();

        public static readonly IImmutableList<string?> WeekDaysValues = WeekDays.GetAll()
            .Prepend(null)
            .ToImmutableList();

        public const string First = "First";
        public const string Second = "Second";
        public const string Third = "Third";
        public const string Fourth = "Fourth";
        public const string Fifth = "Fifth";
        public const string LastDay = "LastDay";

        public static readonly IImmutableList<string?> MonthDaysValues = Enumerable.Range(1, 31)
            .Select(d => d.ToString())
            .Prepend(null)
            .Append(First)
            .Append(Second)
            .Append(Third)
            .Append(Fourth)
            .Append(Fifth)
            .Append(LastDay)
            .ToImmutableList();
    }
}
