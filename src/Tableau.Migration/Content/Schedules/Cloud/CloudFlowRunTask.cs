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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Schedules.Cloud
{
    internal sealed class CloudFlowRunTask : ContentBase, ICloudFlowRunTask
    {
        public string Type { get; set; }

        public int Priority { get; set; }

        public int ConsecutiveFailedCount { get; set; }

        public IContentReference Flow { get; set; }

        public ICloudSchedule Schedule { get; }

        internal CloudFlowRunTask(
            Guid flowTaskId,
            string type,
            int priority,
            int consecutiveFailedCount,
            IContentReference flow,
            ICloudSchedule schedule)
            : base(
                new ContentReferenceStub(
                    flowTaskId,
                    string.Empty,
                    new(flowTaskId.ToString())))
        {
            Type = type;
            Priority = priority;
            ConsecutiveFailedCount = consecutiveFailedCount;
            Flow = flow;
            Schedule = schedule;
        }

        public static async Task<IImmutableList<ICloudFlowRunTask>> CreateManyAsync(
            FlowRunTasksResponse response,
            IContentReferenceFinderFactory finderFactory,
            ILogger logger,
            CancellationToken cancel)
        {
            var items = response.Items
                .Where(t => t.FlowRun != null)
                .Select(t => t.FlowRun!)
                .ToImmutableArray();

            var tasks = ImmutableArray.CreateBuilder<ICloudFlowRunTask>(items.Length);
            var flowFinder = finderFactory.ForContentType<IFlow>();

            foreach (var item in items)
            {
                if (item.Flow?.Id == null || item.Flow.Id == Guid.Empty)
                {
                    logger.LogWarning("Flow reference not found for flow run task {TaskId}", item.Id);
                    continue;
                }

                var flowReference = await flowFinder.FindByIdAsync(item.Flow.Id, cancel).ConfigureAwait(false);

                /*
                 * Flow reference is null when the referenced flow
                 * is in a private space or other "pre-manifest" filter.
                 * 
                 * We similarly filter out those flow run tasks.
                 */
                if (flowReference is not null)
                {
                    // The GET response has frequency and nextRunAt but not frequencyDetails/intervals.
                    // Derive start time and intervals from nextRunAt so the create POST has valid
                    // frequencyDetails (e.g. Monthly needs interval with monthDay, Weekly needs weekDay).
                    var frequency = item.Schedule?.Frequency;
                    if (string.IsNullOrWhiteSpace(frequency))
                    {
                        frequency = ScheduleFrequencies.Daily;
                    }

                    var nextRunAt = item.Schedule?.NextRunAt;
                    var startAt = GetStartAtFromNextRunAt(nextRunAt);
                    var intervals = GetIntervalsFromNextRunAt(frequency, nextRunAt);

                    var schedule = new CloudSchedule(
                        frequency,
                        new FrequencyDetails(startAt, null, intervals));

                    tasks.Add(new CloudFlowRunTask(
                        item.Id,
                        item.Type ?? string.Empty,
                        item.Priority,
                        item.ConsecutiveFailedCount,
                        flowReference,
                        schedule));
                }
            }

            return tasks.ToImmutable();
        }

        public static ICloudFlowRunTask Create(
            CreateCloudFlowTaskResponse.TaskType.FlowRunType response,
            ICloudScheduleType schedule,
            IContentReference flow)
        {
            ICloudSchedule cloudSchedule = schedule.FrequencyDetails is not null
                ? new CloudSchedule(schedule)
                : new CloudSchedule(
                    string.IsNullOrWhiteSpace(schedule.Frequency) ? ScheduleFrequencies.Daily : schedule.Frequency,
                    new FrequencyDetails(null, null, Array.Empty<IInterval>()));

            return new CloudFlowRunTask(
                response.Id,
                response.Type ?? string.Empty,
                response.Priority,
                response.ConsecutiveFailedCount,
                flow,
                cloudSchedule);
        }

        /// <summary>
        /// Derives a schedule start time from the GET response's nextRunAt (e.g. "2026-04-01T01:00:00Z").
        /// Rounds to quarter hour (00, 15, 30, 45 minutes) per Tableau Cloud API requirements.
        /// Returns midnight when nextRunAt is missing or invalid so the create request always has a valid start time.
        /// </summary>
        private static TimeOnly GetStartAtFromNextRunAt(string? nextRunAt)
        {
            if (string.IsNullOrWhiteSpace(nextRunAt))
                return default; // 00:00:00

            if (!DateTimeOffset.TryParse(nextRunAt, out var dt))
                return default;

            var time = TimeOnly.FromDateTime(dt.UtcDateTime);
            // Round to nearest quarter hour (00, 15, 30, 45); seconds must be 0
            int totalMinutes = time.Hour * 60 + time.Minute;
            int roundedQuarters = (int)Math.Round(totalMinutes / 15.0);
            int roundedMinutes = Math.Min(roundedQuarters * 15, 23 * 60 + 45);
            return new TimeOnly(roundedMinutes / 60, roundedMinutes % 60, 0);
        }

        /// <summary>
        /// Derives interval(s) from the GET response's nextRunAt so the create POST includes valid frequencyDetails.
        /// The list endpoint does not return intervals; Cloud create API requires them for Monthly (monthDay), Weekly/Daily (weekDay), Hourly (hours/minutes).
        /// </summary>
        private static IInterval[] GetIntervalsFromNextRunAt(string frequency, string? nextRunAt)
        {
            DateTimeOffset? dt = null;
            if (!string.IsNullOrWhiteSpace(nextRunAt) && DateTimeOffset.TryParse(nextRunAt, out var parsed))
                dt = parsed;

            if (ScheduleFrequencies.IsAMatch(frequency, ScheduleFrequencies.Monthly))
            {
                // Monthly requires at least one interval with monthDay (1-31 or "LastDay").
                if (dt.HasValue)
                {
                    var d = dt.Value.UtcDateTime.Day;
                    var lastDay = DateTime.DaysInMonth(dt.Value.Year, dt.Value.Month);
                    var monthDay = d == lastDay ? "LastDay" : d.ToString();
                    return [Interval.WithMonthDay(monthDay)];
                }
                return [Interval.WithMonthDay("1")];
            }

            if (ScheduleFrequencies.IsAMatch(frequency, ScheduleFrequencies.Weekly) ||
                ScheduleFrequencies.IsAMatch(frequency, ScheduleFrequencies.Daily))
            {
                // Weekly (exactly one) and Daily (at least one) require weekDay.
                var weekDay = dt.HasValue ? DayOfWeekToWeekDay(dt.Value.DayOfWeek) : WeekDays.Monday;
                return [Interval.WithWeekday(weekDay)];
            }

            if (ScheduleFrequencies.IsAMatch(frequency, ScheduleFrequencies.Hourly))
            {
                // Hourly requires one interval with Hours=1 or Minutes=60.
                return [Interval.WithHours(1)];
            }

            // Unknown or unsupported frequency; default to one weekday so validator is satisfied if it expects intervals.
            return [Interval.WithWeekday(WeekDays.Monday)];
        }

        private static string DayOfWeekToWeekDay(DayOfWeek dayOfWeek) => dayOfWeek switch
        {
            DayOfWeek.Sunday => WeekDays.Sunday,
            DayOfWeek.Monday => WeekDays.Monday,
            DayOfWeek.Tuesday => WeekDays.Tuesday,
            DayOfWeek.Wednesday => WeekDays.Wednesday,
            DayOfWeek.Thursday => WeekDays.Thursday,
            DayOfWeek.Friday => WeekDays.Friday,
            DayOfWeek.Saturday => WeekDays.Saturday,
            _ => WeekDays.Monday
        };
    }
}

