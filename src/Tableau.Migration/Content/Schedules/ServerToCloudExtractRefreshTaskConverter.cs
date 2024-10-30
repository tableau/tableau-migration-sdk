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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Schedules
{
    /// <summary>
    /// Converter for converting ServerExtractRefreshTask to CloudExtractRefreshTask.
    /// </summary>
    internal class ServerToCloudExtractRefreshTaskConverter :
        ExtractRefreshTaskConverterBase<IServerExtractRefreshTask, IServerSchedule, ICloudExtractRefreshTask, ICloudSchedule>
    {
        private readonly IScheduleValidator<IServerSchedule> _serverScheduleValidator;
        private readonly IScheduleValidator<ICloudSchedule> _cloudScheduleValidator;

        private ScheduleComparers _scheduleComparers = new ScheduleComparers();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerToCloudExtractRefreshTaskConverter"/> class.
        /// </summary>
        /// <param name="serverScheduleValidator">Validator for server schedule.</param>
        /// <param name="cloudScheduleValidator">Validator for cloud schedule.</param>
        /// <param name="logger">The logger instance for logging.</param>
        /// <param name="localizer">The localizer instance for localization.</param>
        public ServerToCloudExtractRefreshTaskConverter(
            IScheduleValidator<IServerSchedule> serverScheduleValidator,
            IScheduleValidator<ICloudSchedule> cloudScheduleValidator,
            ILogger<ServerToCloudExtractRefreshTaskConverter> logger,
            ISharedResourcesLocalizer localizer)
            : base(logger, localizer)
        {
            _serverScheduleValidator = serverScheduleValidator;
            _cloudScheduleValidator = cloudScheduleValidator;
        }

        /// <summary>
        /// Converts a server extract refresh schedule to a cloud extract refresh schedule.
        /// </summary>
        /// <param name="sourceSchedule">The server schedule to convert.</param>
        /// <returns>The converted cloud schedule.</returns>
        protected override ICloudSchedule ConvertSchedule(IServerSchedule sourceSchedule)
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
                    throw new InvalidScheduleException(Localizer[SharedResourceKeys.FrequencyNotSetError]);

                default:
                    throw new ArgumentException(Localizer[SharedResourceKeys.FrequencyNotSupportedError]);
            }

            _cloudScheduleValidator.Validate(cloudSchedule);

            if (changeMessage.Length > 0)
            {
                // We have schedule updates
                if (_scheduleComparers.Compare(sourceSchedule, cloudSchedule) == 0)
                {
                    // We have updates, but the schedules are the same. Something went wrong.
                    throw new InvalidOperationException(Localizer[SharedResourceKeys.ScheduleUpdateFailedError, sourceSchedule, cloudSchedule]);
                }
                Logger.LogInformation(Localizer[SharedResourceKeys.ScheduleUpdatedMessage, sourceSchedule, cloudSchedule, changeMessage.ToString()]);
            }
            else
            {
                // We have not made updates
                if (_scheduleComparers.Compare(sourceSchedule, cloudSchedule) != 0)
                {
                    // We have not made updates, but the schedules are different. Something went wrong.
                    throw new InvalidOperationException(Localizer[SharedResourceKeys.ScheduleUpdateFailedError, sourceSchedule, cloudSchedule]);
                }
            }

            return cloudSchedule;
        }

        /// <summary>
        /// Creates a new instance of the target extract refresh task.
        /// </summary>
        /// <param name="source">The source extract refresh task.</param>
        /// <param name="targetSchedule">The converted target schedule.</param>
        /// <returns>A new instance of the target extract refresh task.</returns>
        protected override ICloudExtractRefreshTask ConvertExtractRefreshTask(IServerExtractRefreshTask source, ICloudSchedule targetSchedule)
        {
            var type = source.Type;
            if (type == ExtractRefreshType.ServerIncrementalRefresh)
            {
                type = ExtractRefreshType.CloudIncrementalRefresh;
            }

            return new CloudExtractRefreshTask(source.Id, type, source.ContentType, source.Content, targetSchedule);
        }

        private void ConvertHourly(CloudSchedule schedule, StringBuilder changeMessage)
        {
            if (schedule.FrequencyDetails.Intervals.Any(interval => interval.Hours.HasValue && interval.Hours.Value != 1))
            {
                // This is a schedule that should run every n hours, where n is not 1. This means we need to convert it to a daily schedule.
                schedule.Frequency = ScheduleFrequencies.Daily;
                changeMessage.AppendLine(Localizer[SharedResourceKeys.ScheduleUpdatedFrequencyToDailyMessage]);
                ConvertDaily(schedule, changeMessage);
                return;
            }

            // If schedule has no weekday intervals, add all weekdays
            if (!schedule.FrequencyDetails.Intervals.Any(interval => !interval.WeekDay.IsNullOrEmpty()))
            {
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Monday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Tuesday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Wednesday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Thursday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Friday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Saturday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Sunday"));
                changeMessage.AppendLine(Localizer[SharedResourceKeys.ScheduleUpdatedAddedWeekdayMessage]);
            }


            if (schedule.FrequencyDetails.Intervals.Any(interval => interval.Hours.HasValue || interval.Minutes.HasValue))
            {
                // If the schedule has an interval with hours, but the hours are not 1, then it was caught earlier and we don't need to deal with it here

                var invalidMinuteIntervals = schedule.FrequencyDetails.Intervals.Where(interval => interval.Minutes.HasValue && interval.Minutes.Value != 60).ToList();
                if (invalidMinuteIntervals.Any())
                {
                    // We have invalid minute intervals
                    foreach (var interval in invalidMinuteIntervals)
                    {
                        schedule.FrequencyDetails.Intervals.Remove(interval);
                    }
                    schedule.FrequencyDetails.Intervals.Add(Interval.WithMinutes(60));
                    changeMessage.AppendLine(Localizer[SharedResourceKeys.ReplacingHourlyIntervalMessage]);
                }

                return;
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
            if (schedule.FrequencyDetails.Intervals.Where(i => i.Hours.HasValue).ToList().Count > 1)
            {
                schedule.FrequencyDetails.Intervals = new List<IInterval>() { schedule.FrequencyDetails.Intervals.Last() };
                changeMessage.AppendLine(Localizer[SharedResourceKeys.ScheduleShouldOnlyHaveOneHoursIntervalWarning, schedule]);
            }

            // Validate that the hours interval is one of the valid values
            var hourInterval = schedule.FrequencyDetails.Intervals.Where(i => i.Hours.HasValue).LastOrDefault();
            if (hourInterval is not null)
            {
                if (!IsValidHour(hourInterval.Hours!.Value))
                {
                    var newHourInterval = Interval.WithHours(FindNearestValidHour(hourInterval.Hours!.Value));
                    schedule.FrequencyDetails.Intervals.Remove(hourInterval);
                    schedule.FrequencyDetails.Intervals.Add(newHourInterval);
                    changeMessage.AppendLine(Localizer[SharedResourceKeys.ScheduleUpdatedHoursMessage, hourInterval.Hours, newHourInterval.Hours!]);
                }
            }

            if (schedule.FrequencyDetails.Intervals.Any(interval => interval.Hours.HasValue))
            {
                // We have intervals with hours
                if (schedule.FrequencyDetails.EndAt is null)
                {
                    // End is always required if hours are set
                    schedule.FrequencyDetails.EndAt = schedule.FrequencyDetails.StartAt; // Ending 24h after start
                    changeMessage.AppendLine(Localizer[SharedResourceKeys.ScheduleUpdatedAddedEndAtMessage]);
                }
            }
            else
            {
                // We have no intervals with hours, remove end time
                if (schedule.FrequencyDetails.EndAt is not null)
                {
                    schedule.FrequencyDetails.EndAt = null;
                    changeMessage.AppendLine(Localizer[SharedResourceKeys.ScheduleUpdatedRemovedEndAtMessage]);
                }
            }

            // If schedule has no weekday intervals, add all weekdays
            if (!schedule.FrequencyDetails.Intervals.Any(interval => !interval.WeekDay.IsNullOrEmpty()))
            {
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Monday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Tuesday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Wednesday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Thursday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Friday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Saturday"));
                schedule.FrequencyDetails.Intervals.Add(Interval.WithWeekday("Sunday"));
                changeMessage.AppendLine(Localizer[SharedResourceKeys.ScheduleUpdatedAddedWeekdayMessage]);
                return;
            }
        }

        private void ConvertWeekly(CloudSchedule schedule, StringBuilder changeMessage)
        {
            if (schedule.FrequencyDetails.Intervals.Count(interval => !interval.WeekDay.IsNullOrEmpty()) > 1)
            {
                // We have more than 1 weekday interval in a weekly schedule. This must be converted to Daily.
                schedule.Frequency = ScheduleFrequencies.Daily;
                changeMessage.AppendLine(Localizer[SharedResourceKeys.ScheduleUpdatedFrequencyToDailyMessage]);
                ConvertDaily(schedule, changeMessage);
            }
        }

        public bool IsValidHour(int hour)
        {
            return IntervalValues.CloudHoursValues.Contains(hour);
        }

        public int FindNearestValidHour(int hour)
        {
            return IntervalValues.CloudHoursValues
                .OrderBy(h => Math.Abs(h - hour))
                .FirstOrDefault();
        }
    }
}
