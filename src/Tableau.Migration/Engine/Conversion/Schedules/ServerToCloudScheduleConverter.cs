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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Conversion.Schedules
{
    /// <summary>
    /// Class to convert a server extract refresh schedule to a cloud extract refresh schedule.
    /// </summary>
    internal class ServerToCloudScheduleConverter : IScheduleConverter<IServerSchedule, ICloudSchedule>
    {
        private readonly IScheduleValidator<IServerSchedule> _serverScheduleValidator;
        private readonly IScheduleValidator<ICloudSchedule> _cloudScheduleValidator;
        private readonly ILogger<ServerToCloudScheduleConverter> _logger;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ScheduleComparers _scheduleComparers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerToCloudScheduleConverter"/> class.
        /// </summary>
        /// <param name="serverScheduleValidator">Validator for server schedule.</param>
        /// <param name="cloudScheduleValidator">Validator for cloud schedule.</param>
        /// <param name="logger">The logger instance for logging.</param>
        /// <param name="localizer">The localizer instance for localization.</param>
        public ServerToCloudScheduleConverter(
            IScheduleValidator<IServerSchedule> serverScheduleValidator,
            IScheduleValidator<ICloudSchedule> cloudScheduleValidator,
            ILogger<ServerToCloudScheduleConverter> logger,
            ISharedResourcesLocalizer localizer)
        {
            _serverScheduleValidator = serverScheduleValidator;
            _cloudScheduleValidator = cloudScheduleValidator;
            _logger = logger;
            _localizer = localizer;
        }

        /// <inheritdoc />
        public Task<ICloudSchedule> ConvertAsync(IServerSchedule sourceSchedule, CancellationToken cancel)
        {
            _serverScheduleValidator.Validate(sourceSchedule);

            var cloudSchedule = new CloudSchedule(sourceSchedule);

            var changeMessage = new StringBuilder();

            switch (cloudSchedule.Frequency)
            {
                case ScheduleFrequencies.Hourly:
                    ConvertHourly(cloudSchedule, changeMessage);
                    break;

                case ScheduleFrequencies.Daily:
                    ConvertDaily(cloudSchedule, changeMessage);
                    break;

                case ScheduleFrequencies.Weekly:
                    ConvertWeekly(cloudSchedule, changeMessage);
                    break;

                case ScheduleFrequencies.Monthly:
                    // Monthly schedules require no conversion
                    break;

                case null:
                case "":
                    throw new InvalidScheduleException(_localizer[SharedResourceKeys.FrequencyNotSetError]);

                default:
                    throw new ArgumentException(_localizer[SharedResourceKeys.FrequencyNotSupportedError]);
            }

            _cloudScheduleValidator.Validate(cloudSchedule);

            HandleConversionResult(sourceSchedule, cloudSchedule, changeMessage);

            return Task.FromResult<ICloudSchedule>(cloudSchedule);
        }

        private void ConvertHourly(CloudSchedule schedule, StringBuilder changeMessage)
        {
            if (schedule.FrequencyDetails.Intervals.Any(interval => interval.Hours.HasValue && interval.Hours.Value != 1))
            {
                // This is a schedule that should run every n hours, where n is not 1. This means we need to convert it to a daily schedule.
                schedule.Frequency = ScheduleFrequencies.Daily;
                changeMessage.AppendLine(_localizer[SharedResourceKeys.ScheduleUpdatedFrequencyToDailyMessage]);
                ConvertDaily(schedule, changeMessage);
                return;
            }

            if (!HasWeekdayIntervals(schedule.FrequencyDetails.Intervals))
            {
                AddAllWeekdays(schedule);
                changeMessage.AppendLine(_localizer[SharedResourceKeys.ScheduleUpdatedAddedWeekdayMessage]);
            }

            if (schedule.FrequencyDetails.Intervals.Any(interval => interval.Hours.HasValue || interval.Minutes.HasValue))
            {
                // If the schedule has an interval with hours, but the hours are not 1, then it was caught earlier and we don't need to deal with it here

                var invalidMinuteIntervals = schedule.FrequencyDetails.Intervals.Where(interval => interval.Minutes.HasValue && interval.Minutes.Value != 60).ToList();

                if (invalidMinuteIntervals.Count == 0)
                {
                    return;
                }

                // We have invalid minute intervals
                foreach (var interval in invalidMinuteIntervals)
                {
                    schedule.FrequencyDetails.Intervals.Remove(interval);
                }
                schedule.FrequencyDetails.Intervals.Add(Interval.WithMinutes(60));
                changeMessage.AppendLine(_localizer[SharedResourceKeys.ReplacingHourlyIntervalMessage]);
            }
        }

        private void ConvertDaily(CloudSchedule schedule, StringBuilder changeMessage)
        {
            // Hours is not required
            // If hours exist, only the last one is used
            // if hours exist, it must be 2,4,6,8,12,24
            // if hours exist, and end time must be provided
            // if hours does not exist, end time must not exist

            // If there are more then 1 hours interval, remove all but the last one
            var hourlyIntervals = schedule.FrequencyDetails.Intervals.Where(i => i.Hours.HasValue);
            if (hourlyIntervals.Count() > 1)
            {
                schedule.FrequencyDetails.Intervals = [schedule.FrequencyDetails.Intervals.Last()];
                changeMessage.AppendLine(_localizer[SharedResourceKeys.ScheduleShouldOnlyHaveOneHoursIntervalWarning, schedule]);
            }

            // Validate that the hours interval is one of the valid values
            var hourInterval = hourlyIntervals.LastOrDefault();
            if (hourInterval is not null)
            {
                if (!IsValidHour(hourInterval.Hours!.Value))
                {
                    var newHourInterval = Interval.WithHours(FindNearestValidHour(hourInterval.Hours!.Value));
                    schedule.FrequencyDetails.Intervals.Remove(hourInterval);
                    schedule.FrequencyDetails.Intervals.Add(newHourInterval);
                    changeMessage.AppendLine(_localizer[SharedResourceKeys.ScheduleUpdatedHoursMessage, hourInterval.Hours, newHourInterval.Hours!]);
                }
            }

            if (schedule.FrequencyDetails.Intervals.Any(interval => interval.Hours.HasValue))
            {
                // We have intervals with hours
                if (schedule.FrequencyDetails.EndAt is null)
                {
                    // End is always required if hours are set
                    schedule.FrequencyDetails.EndAt = schedule.FrequencyDetails.StartAt; // Ending 24h after start
                    changeMessage.AppendLine(_localizer[SharedResourceKeys.ScheduleUpdatedAddedEndAtMessage]);
                }
            }
            else
            {
                // We have no intervals with hours, remove end time
                if (schedule.FrequencyDetails.EndAt is not null)
                {
                    schedule.FrequencyDetails.EndAt = null;
                    changeMessage.AppendLine(_localizer[SharedResourceKeys.ScheduleUpdatedRemovedEndAtMessage]);
                }
            }

            if (!HasWeekdayIntervals(schedule.FrequencyDetails.Intervals))
            {
                AddAllWeekdays(schedule);
                changeMessage.AppendLine(_localizer[SharedResourceKeys.ScheduleUpdatedAddedWeekdayMessage]);
            }
        }

        private void HandleConversionResult(IServerSchedule sourceSchedule, CloudSchedule cloudSchedule, StringBuilder changeMessage)
        {
            if (changeMessage.Length > 0)
            {
                // We have schedule updates
                if (_scheduleComparers.Compare(sourceSchedule, cloudSchedule) == 0)
                {
                    // We have updates, but the schedules are the same. Something went wrong.
                    throw new InvalidOperationException(_localizer[SharedResourceKeys.ScheduleUpdateFailedError, sourceSchedule, cloudSchedule]);
                }
                _logger.LogInformation(_localizer[SharedResourceKeys.ScheduleUpdatedMessage, sourceSchedule, cloudSchedule, changeMessage.ToString()]);
                return;
            }

            // We have not made updates
            if (_scheduleComparers.Compare(sourceSchedule, cloudSchedule) != 0)
            {
                // We have not made updates, but the schedules are different. Something went wrong.
                throw new InvalidOperationException(_localizer[SharedResourceKeys.ScheduleUpdateFailedError, sourceSchedule, cloudSchedule]);
            }
        }

        private static void AddAllWeekdays(CloudSchedule schedule)
        {
            schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday(WeekDays.Monday));
            schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday(WeekDays.Tuesday));
            schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday(WeekDays.Wednesday));
            schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday(WeekDays.Thursday));
            schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday(WeekDays.Friday));
            schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday(WeekDays.Saturday));
            schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday(WeekDays.Sunday));
        }

        private void ConvertWeekly(CloudSchedule schedule, StringBuilder changeMessage)
        {
            if (HasWeekdayIntervals(schedule.FrequencyDetails.Intervals))
            {
                // We have more than 1 weekday interval in a weekly schedule. This must be converted to Daily.
                schedule.Frequency = ScheduleFrequencies.Daily;
                changeMessage.AppendLine(_localizer[SharedResourceKeys.ScheduleUpdatedFrequencyToDailyMessage]);
                ConvertDaily(schedule, changeMessage);
            }
        }

        private static bool HasWeekdayIntervals(IList<IInterval> intervals)
            => intervals.Count(interval => !interval.WeekDay.IsNullOrEmpty()) > 1;

        public static bool IsValidHour(int hour) => IntervalValues.CloudHoursValues.Contains(hour);

        public static int FindNearestValidHour(int hour)
            => IntervalValues.CloudHoursValues.OrderBy(h => Math.Abs(h - hour)).FirstOrDefault();
    }
}
