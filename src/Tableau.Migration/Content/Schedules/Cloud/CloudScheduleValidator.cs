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
using System.Linq;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Schedules.Cloud
{
    internal class CloudScheduleValidator : IScheduleValidator<ICloudSchedule>
    {
        private readonly ILogger<CloudScheduleValidator> _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudScheduleValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging.</param>
        /// <param name="localizer">The localizer instance for localization.</param>
        public CloudScheduleValidator(ILogger<CloudScheduleValidator> logger, ISharedResourcesLocalizer localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// Validates that the cloud schedule is valid based on the required of the "Create Cloud Extract Refresh Task" RestAPI.
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_extract_and_encryption.htm#create_cloud_extract_refresh_task">Tableau API Reference</see> for documentation.
        /// </summary>
        /// <param name="schedule">Cloud schedule to validate.</param>
        public void Validate(ICloudSchedule schedule)
        {
            //
            // This validation process is not exhaustive. Additional checks may be required to ensure complete validity.
            //

            Guard.AgainstNull(schedule, nameof(schedule));

            switch (schedule.Frequency)
            {
                case ScheduleFrequencies.Hourly:
                    ValidateHourly(schedule);
                    break;
                case ScheduleFrequencies.Daily:
                    ValidateDaily(schedule);
                    break;
                case ScheduleFrequencies.Weekly:
                    ValidateWeekly(schedule);
                    break;
                case ScheduleFrequencies.Monthly:
                    ValidateMonthly(schedule);
                    break;

                case null:
                case "":
                    LogAndThrow(schedule, new ArgumentException(_localizer[SharedResourceKeys.FrequencyNotSetError]));
                    break;

                default:
                    LogAndThrow(schedule, new ArgumentException(_localizer[SharedResourceKeys.FrequencyNotSupportedError]));
                    break;
            }
        }

        private void LogAndThrow(ICloudSchedule schedule, Exception exception)
        {
            _logger.LogError(exception, _localizer[SharedResourceKeys.InvalidScheduleError, schedule]);
            throw exception;
        }

        #region - Frequency Specific Validation -

        private void ValidateHourly(ICloudSchedule schedule)
        {
            ValidateExpectedFrequency(schedule, ScheduleFrequencies.Hourly);

            ValidateStartTime(schedule);

            ValidateEndTime(schedule);

            ValidateTimeDifference(schedule);

            ValidateHourOrMinuteIsSetOnce(schedule);

            ValidateAtLeastOneWeekday(schedule);

            ValidateWeekDayValues(schedule);
        }

        private void ValidateDaily(ICloudSchedule schedule)
        {
            ValidateExpectedFrequency(schedule, ScheduleFrequencies.Daily);

            ValidateStartTime(schedule);

            ValidateAtLeastOneWeekday(schedule);

            ValidateWeekDayValues(schedule);

            // Hours is not required
            // If hours exist, only the last one is used
            // if hours exist, it must be 2,4,6,8,12,24

            var intervals = schedule.FrequencyDetails.Intervals;

            // Validate that there is exactly one hours interval
            var hoursIntervals = intervals.Where(i => i.Hours.HasValue).ToList();
            if (hoursIntervals.Count > 1)
            {
#pragma warning disable CA2254 // Template should be a static expression - Suppressing as this message is used outside of logging as well
                _logger.LogWarning(_localizer[SharedResourceKeys.ScheduleShouldOnlyHaveOneHoursIntervalWarning, schedule]);
#pragma warning restore CA2254 // Template should be a static expression
            }

            // Validate that the hours interval is one of the valid values
            var hourInterval = intervals.Where(i => i.Hours.HasValue).LastOrDefault();
            if (hourInterval is not null && !IsValidHours(hourInterval.Hours!.Value))
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.InvalidHourlyIntervalForCloudError, schedule.Frequency]));
            }



            if (hourInterval is not null)
            {
                // Note: Docs say that if hours is less then 24, then end is required.
                // However, when actually trying this, any value where hours has a valid requires an end time.
                ValidateEndTime(schedule);

                ValidateTimeDifference(schedule);
            }
            else
            {
                // If hours does not exist, then end must not exist
                ValidateNoEndTime(schedule);
            }
        }

        private void ValidateWeekly(ICloudSchedule schedule)
        {
            ValidateExpectedFrequency(schedule, ScheduleFrequencies.Weekly);

            ValidateStartTime(schedule);

            var intervals = schedule.FrequencyDetails.Intervals;

            // Validate that there is exactly one weekday interval
            var weekDayIntervals = intervals.Where(i => !string.IsNullOrEmpty(i.WeekDay)).ToList();
            if (weekDayIntervals.Count != 1)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ScheduleMustHaveExactlyOneWeekdayIntervalError, schedule.Frequency]));
            }

            // Validate that the weekday is one of the valid values
            var weekDay = weekDayIntervals.First().WeekDay!;
            if (!IsValidWeekDay(weekDay))
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.InvalidWeekdayError]));
            }

            ValidateNoEndTime(schedule);
        }

        private void ValidateMonthly(ICloudSchedule schedule)
        {
            ValidateExpectedFrequency(schedule, ScheduleFrequencies.Monthly);

            ValidateStartTime(schedule);

            var intervals = schedule.FrequencyDetails.Intervals;

            // Validate that there is at least one interval
            if (!intervals.Any())
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.AtLeastOneIntervalError, schedule.Frequency]));
            }

            // Validate that at least one interval has MonthDay set
            if (!intervals.Any(i => !string.IsNullOrEmpty(i.MonthDay)))
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.AtLeastOneValidMonthDayError]));
            }

            foreach (var interval in intervals)
            {
                // Validate monthDay by day number
                if (!string.IsNullOrEmpty(interval.MonthDay) && IsValidMonthDayNumber(interval.MonthDay))
                {
                    if (interval.MonthDay == IntervalValues.OccurrenceOfWeekday.LastDay.ToString() && intervals.Count(i => i.MonthDay == IntervalValues.OccurrenceOfWeekday.LastDay.ToString()) > 1)
                    {
                        LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ScheduleMustOnlyHaveOneIntervalWithLastDayError, schedule.Frequency]));
                    }
                }
                // Validate monthDay by occurrence of weekday
                else if (!string.IsNullOrEmpty(interval.MonthDay) && IsValidMonthDayOccurrence(interval.MonthDay) && IsValidWeekDay(interval.WeekDay))
                {
                    if (interval.MonthDay == IntervalValues.OccurrenceOfWeekday.LastDay.ToString() && intervals.Count(i => i.MonthDay == IntervalValues.OccurrenceOfWeekday.LastDay.ToString() && i.WeekDay == interval.WeekDay) > 1)
                    {
                        LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ScheduleMustOnlyHaveOneIntervalWithLastDayError, schedule.Frequency]));
                    }
                }
                else
                {
                    LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.InvalidScheduleForMonthlyError]));
                }
            }

            // Ensure there is no end time specified
            ValidateNoEndTime(schedule);
        }

        #endregion

        #region - Validation Helper Methods -
        private void ValidateExpectedFrequency(ICloudSchedule schedule, string expectedFrequency)
        {
            if (!ScheduleFrequencies.IsAMatch(schedule.Frequency, expectedFrequency))
            {
                LogAndThrow(schedule, new ArgumentException(_localizer[SharedResourceKeys.FrequencyNotSetError]));
            }
        }

        private void ValidateStartTime(ICloudSchedule schedule)
        {
            // Docs say that start time can be anything, but API responds states it must be quarter hour.
            // In reality, 5 minute increments are all that can be set via RestAPI and UI, and those work. 
            // No need to check anything.

            if (!schedule.FrequencyDetails.StartAt.HasValue)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ScheduleMustHaveStartAtTimeError, schedule.Frequency]));
            }
        }

        private void ValidateEndTime(ICloudSchedule schedule)
        {
            // Docs say that start time can be anything, but API responds states it must be quarter hour.
            // In reality, 5 minute increments are all that can be set via RestAPI and UI, and those work. 
            // No need to check anything.

            if (!schedule.FrequencyDetails.EndAt.HasValue)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ScheduleMustHaveEndAtTimeError, schedule.Frequency]));
            }
        }

        private void ValidateNoEndTime(ICloudSchedule schedule)
        {
            if (schedule.FrequencyDetails.EndAt.HasValue)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ScheduleMustNotHaveEndAtTimeError, schedule.Frequency]));
            }
        }

        private void ValidateAtLeastOneWeekday(ICloudSchedule schedule)
        {
            if (!schedule.FrequencyDetails.Intervals.Any(interval => !string.IsNullOrEmpty(interval.WeekDay)))
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.AtLeastOneValidWeekdayError, schedule.Frequency]));
            }
        }
        private void ValidateHourOrMinuteIsSetOnce(ICloudSchedule schedule)
        {
            var intervals = schedule.FrequencyDetails.Intervals;

            // Check that there is exactly one interval where either hours or minutes is set, but not both
            var validIntervals = intervals.Where(i => (i.Hours.HasValue && !i.Minutes.HasValue) || (!i.Hours.HasValue && i.Minutes.HasValue)).ToList();

            if (validIntervals.Count != 1)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ExactlyOneHourOrMinutesError]));
            }

            var interval = validIntervals.First();

            // Check that if hours is set, it must be 1
            if (interval.Hours.HasValue && interval.Hours != 1)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.IntervalMustBe1HourOr60MinError, schedule.Frequency]));
            }

            // Check that if minutes is set, it must be 60
            if (interval.Minutes.HasValue && interval.Minutes != 60)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.IntervalMustBe1HourOr60MinError, schedule.Frequency]));
            }
        }

        private void ValidateWeekDayValues(ICloudSchedule schedule)
        {
            if (schedule.FrequencyDetails.Intervals.Any(interval => !string.IsNullOrEmpty(interval.WeekDay) && !IsValidWeekDay(interval.WeekDay)))
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.InvalidWeekdayError]));

            }
        }

        private bool IsValidWeekDay(string? weekDay)
        {
            return IntervalValues.WeekDaysValues.Contains(weekDay);
        }

        private bool IsValidHours(int hours)
        {
            return IntervalValues.CloudHoursValues.Contains(hours);
        }

        private bool IsValidMonthDayNumber(string? monthDay)
        {
            return IntervalValues.MonthDayNumberValues.Contains(monthDay);
        }

        private bool IsValidMonthDayOccurrence(string? occurrence)
        {
            return IntervalValues.MonthDayOccurrenceValues.Contains(occurrence);
        }

        /// <summary>
        /// Validates that the time difference is exactly 60 minutes.
        /// </summary>
        private void ValidateTimeDifference(ICloudSchedule schedule)
        {
            var startTime = schedule.FrequencyDetails.StartAt!.Value;
            var endTime = schedule.FrequencyDetails.EndAt!.Value;

            var timeDifference = (endTime - startTime).TotalMinutes;

            if (timeDifference % 60 != 0)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.StartEndTimeDifferenceError]));
            }
        }

        #endregion


    }
}
