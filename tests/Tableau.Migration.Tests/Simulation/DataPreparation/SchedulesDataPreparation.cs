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
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing schedules data for migration tests.
    /// </summary>
    public static class SchedulesDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <returns>The list of prepared schedules.</returns>
        public static List<ScheduleResponse.ScheduleType> PrepareServerSource(
            TableauApiSimulator sourceApi)
        {
            var hourlySchedule = new ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = ScheduleFrequencies.Hourly,
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Hourly,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "00:25:00",
                    End = "01:25:00",
                    Intervals = [
                        new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { Hours = "1" },
                    ]
                }
            };
            var dailySchedule = new ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = ScheduleFrequencies.Daily,
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Daily,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "00:15:00",
                    End = "12:15:00",
                    Intervals = [
                        new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { Hours = "12" },
                        new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Tuesday },
                        new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Thursday }
                    ]
                }
            };
            var weeklySchedule = new ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = ScheduleFrequencies.Weekly,
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Weekly,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "03:10:00",
                    Intervals = [new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Sunday }]
                }
            };
            var monthlyMultipleDaysSchedule = new ScheduleResponse.ScheduleType
            {
                // Note: This type of schedule on Server can only be created via the UI, not the RestAPI. It is still a valid schedule though. 
                Id = Guid.NewGuid(),
                Name = $"{ScheduleFrequencies.Monthly}_Multiple",
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Monthly,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "03:45:00",
                    Intervals = [
                        new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { MonthDay = "1" },
                        new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { MonthDay = "10" },
                        new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { MonthDay = "20" }
                    ]
                }
            };
            var monthlyLastDaySchedule = new ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = $"{ScheduleFrequencies.Monthly}_LastSunday",
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Monthly,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "01:35:00",
                    Intervals = [new ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { MonthDay = "LastDay" }]
                }
            };
            var schedules = new List<ScheduleResponse.ScheduleType>
            {
                hourlySchedule,
                dailySchedule,
                weeklySchedule,
                monthlyMultipleDaysSchedule,
                monthlyLastDaySchedule
            };

            foreach (var schedule in schedules)
            {
                sourceApi.Data.AddSchedule(schedule);
            }

            return schedules;
        }
    }
}