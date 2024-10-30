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

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Schedules.Server
{
    internal class ServerScheduleValidator : IScheduleValidator<IServerSchedule>
    {
        private readonly ILogger<ServerScheduleValidator> _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerScheduleValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging.</param>
        /// <param name="localizer">The localizer instance for localization.</param>
        public ServerScheduleValidator(ILogger<ServerScheduleValidator> logger, ISharedResourcesLocalizer localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// Validates that the server schedule is valid based on the required of the "Create Server Schedule" RestAPI.
        /// https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_jobs_tasks_and_schedules.htm#create_schedule
        /// </summary>
        /// <param name="schedule">Server schedule to validate.</param>
        public void Validate(IServerSchedule schedule)
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
                    LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.FrequencyNotSetError]));
                    break;

                default:
                    LogAndThrow(schedule, new ArgumentException(_localizer[SharedResourceKeys.FrequencyNotSupportedError]));
                    break;
            }
        }
        private void LogAndThrow(IServerSchedule schedule, Exception exception)
        {
            var scheduleStr = schedule.ToString();

            _logger.LogError(exception, _localizer[SharedResourceKeys.InvalidScheduleError, schedule]);
            throw exception;
        }

        private void ValidateHourly(IServerSchedule schedule)
        {
            ValidateExpectedFrequency(schedule, ScheduleFrequencies.Hourly);

            ValidateStartTime(schedule);

            ValidateAtLeastOneIntervalWithHoursOrMinutes(schedule);

            ValidateEndTime(schedule);

            foreach (var interval in schedule.FrequencyDetails.Intervals)
            {
                if (interval.Hours.HasValue && interval.Minutes.HasValue)
                {
                    LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.BothHoursAndMinutesIntervalError]));
                }

                if (interval.Minutes.HasValue)
                {
                    // Documentation states that only 15 and 30 minutes are supported.
                    // Testing shows that 60 minutes is also valid.
                    var minutes = interval.Minutes.Value;
                    if (minutes != 15 && minutes != 30 && minutes != 60)
                    {
                        _logger.LogWarning(_localizer[SharedResourceKeys.InvalidMinuteIntervalWarning], schedule);
                    }
                }
            }
        }

        private void ValidateDaily(IServerSchedule schedule)
        {
            ValidateExpectedFrequency(schedule, ScheduleFrequencies.Daily);

            ValidateStartTime(schedule);

            if (schedule.FrequencyDetails.Intervals.Count() > 0)
            {
                _logger.LogWarning(_localizer[SharedResourceKeys.IntervalsIgnoredWarning, schedule.Frequency]);
            }

        }

        private void ValidateWeekly(IServerSchedule schedule)
        {
            ValidateExpectedFrequency(schedule, ScheduleFrequencies.Weekly);

            ValidateStartTime(schedule);

            // Validate that there are between 1 and 7 intervals
            if (!(schedule.FrequencyDetails.Intervals.Count() >= 1 && schedule.FrequencyDetails.Intervals.Count() <= 7))
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.WeeklyScheduleIntervalError]));
            }

            // Validate that weekday intervals are valid
            if (schedule.FrequencyDetails.Intervals.Any(interval => !interval.WeekDay.IsNullOrEmpty() && !IsValidWeekDay(interval.WeekDay)))
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.InvalidWeekdayError]));
            }
        }

        private void ValidateMonthly(IServerSchedule schedule)
        {
            ValidateExpectedFrequency(schedule, ScheduleFrequencies.Monthly);

            ValidateStartTime(schedule);

            ValidateAtLeastOneInterval(schedule);

            // Checks that intervals with MonthDay set are either "LastDay" or a number between 1 and 31
            if (schedule.FrequencyDetails.Intervals
                .Where(interval => interval.MonthDay != "LastDay")
                .Any(interval => !int.TryParse(interval.MonthDay, out var monthDay) || monthDay < 1 || monthDay > 31))
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.InvalidMonthDayError]));
            }

        }

        #region - Validation Helper Methods -
        private void ValidateExpectedFrequency(IServerSchedule schedule, string expectedFrequency)
        {
            if (!ScheduleFrequencies.IsAMatch(schedule.Frequency, expectedFrequency))
            {
                LogAndThrow(schedule, new ArgumentException(_localizer[SharedResourceKeys.FrequencyNotExpectedError, expectedFrequency]));
            }
        }


        private void ValidateStartTime(IServerSchedule schedule)
        {
            // Docs say that start time can be anything, but API responds states it must be quarter hour.
            // In reality, 5 minute increments are all that can be set via RestAPI and UI, and those work. 
            // No need to check anything.

            if (!schedule.FrequencyDetails.StartAt.HasValue)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ScheduleMustHaveStartAtTimeError, schedule.Frequency]));
            }
        }


        private void ValidateEndTime(IServerSchedule schedule)
        {
            // Docs say that start time can be anything, but API responds states it must be quarter hour.
            // In reality, 5 minute increments are all that can be set via RestAPI and UI, and those work. 
            // No need to check anything.

            if (!schedule.FrequencyDetails.EndAt.HasValue)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.ScheduleMustHaveEndAtTimeError, schedule.Frequency]));
            }
        }

        private void ValidateAtLeastOneInterval(IServerSchedule schedule)
        {
            // Validate that there is at least one interval
            if (schedule.FrequencyDetails.Intervals.Count() <= 0)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.AtLeastOneIntervalError, schedule.Frequency]));
            }
        }

        private void ValidateAtLeastOneIntervalWithHoursOrMinutes(IServerSchedule schedule)
        {
            // Validate that there is at least one interval
            if (schedule.FrequencyDetails.Intervals.Count(interval => interval.Hours.HasValue || interval.Minutes.HasValue) <= 0)
            {
                LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.AtLeastOneIntervalWithHourOrMinutesError, schedule.Frequency]));
            }

            var intervals = schedule.FrequencyDetails.Intervals.Where(interval => interval.Hours.HasValue || interval.Minutes.HasValue);
            foreach (var interval in intervals)
            {
                if (interval.Hours.HasValue && !IsValidHour(interval.Hours.Value))
                {
                    LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.InvalidHourlyIntervalForServerError]));
                }

                if (interval.Minutes.HasValue && !IsValidMinutes(interval.Minutes.Value))
                {
                    LogAndThrow(schedule, new InvalidScheduleException(_localizer[SharedResourceKeys.InvalidMinuteIntervalError]));
                }
            }
        }

        private bool IsValidWeekDay(string? weekDay)
        {
            return IntervalValues.WeekDaysValues.Contains(weekDay);
        }

        private bool IsValidHour(int hour)
        {
            return IntervalValues.ServerHoursValues.Contains(hour);
        }

        private bool IsValidMinutes(int minutes)
        {
            if (minutes == 15 || minutes == 30)
                return true;

            return false;
        }

        #endregion
    }
}
