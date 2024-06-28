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

using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content.Schedules
{
    internal readonly record struct Interval : IInterval
    {
        public int? Hours { get; }
        public int? Minutes { get; }
        public string? WeekDay { get; }
        public string? MonthDay { get; }

        public Interval(IScheduleIntervalType response)
            : this(
                  response.Hours.ToIntOrNull(),
                  response.Minutes.ToIntOrNull(),
                  response.WeekDay,
                  response.MonthDay)
        { }

        internal Interval(
            int? hours = null, 
            int? minutes = null, 
            string? weekDay = null, 
            string? monthDay = null)
        {
            Hours = hours;
            Minutes = minutes;
            WeekDay = NormalizeValue(weekDay);
            MonthDay = NormalizeValue(monthDay);
        }

        #region - Factory Methods -

        public static IInterval WithHours(int hours) => new Interval(hours: hours);
        public static IInterval WithMinutes(int minutes) => new Interval(minutes: minutes);
        public static IInterval WithWeekday(string weekDay) => new Interval(weekDay: weekDay);
        public static IInterval WithMonthDay(string monthDay) => new Interval(monthDay: monthDay);
        public static IInterval WithWeekdayMonthDay(string weekDay, string monthDay) => new Interval(weekDay: weekDay, monthDay: monthDay);

        #endregion

        private static string? NormalizeValue(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
    }
}