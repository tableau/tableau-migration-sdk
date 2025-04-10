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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.ContentConverters.Schedules;
using Tableau.Migration.Engine.Conversion.Schedules;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Conversion.Schedules
{
    public class ServerToCloudScheduleConverterTests : AutoFixtureTestBase
    {
        private readonly Mock<ILogger<ServerScheduleValidator>> _mockServerScheduleValidatorLogger;
        private readonly Mock<ILogger<CloudScheduleValidator>> _mockCloudScheduleValidatorLogger;
        private readonly Mock<ILogger<ServerToCloudExtractRefreshTaskConverter>> _mockConverterLogger;
        private readonly Mock<ILogger<ServerToCloudScheduleConverter>> _mockScheduleConverterLogger;
        private readonly ISharedResourcesLocalizer _localizer;

        private readonly IScheduleConverter<IServerSchedule, ICloudSchedule> Converter;
        public static TheoryData<IServerSchedule, ICloudSchedule> ValidSchedules = new();

        /// <summary>
        /// Static constructor to create the test data
        /// </summary>
        static ServerToCloudScheduleConverterTests()
        {
            CreateValidSchedules();
        }

        public ServerToCloudScheduleConverterTests()
        {
            CreateValidSchedules();

            // Create the real localizer
            var services = new ServiceCollection();
            services.AddTableauMigrationSdk();
            var container = services.BuildServiceProvider();

            _mockServerScheduleValidatorLogger = new Mock<ILogger<ServerScheduleValidator>>();
            _mockCloudScheduleValidatorLogger = new Mock<ILogger<CloudScheduleValidator>>();
            _mockConverterLogger = new Mock<ILogger<ServerToCloudExtractRefreshTaskConverter>>();
            _mockScheduleConverterLogger = new Mock<ILogger<ServerToCloudScheduleConverter>>();
            _localizer = container.GetRequiredService<ISharedResourcesLocalizer>();

            var serverValidator = new ServerScheduleValidator(_mockServerScheduleValidatorLogger.Object, _localizer);
            var cloudValidator = new CloudScheduleValidator(_mockCloudScheduleValidatorLogger.Object, _localizer);
            Converter = new ServerToCloudScheduleConverter(serverValidator, cloudValidator, _mockScheduleConverterLogger.Object, _localizer);
        }

        [Theory]
        [MemberData(nameof(ValidSchedules))]
        public async Task ValidScheduleAsync(IServerSchedule input, ICloudSchedule expectedOutput)
        {
            // Act
            var result = await Converter.ConvertAsync(input, Cancel);

            // Assert
            Assert.Equal(expectedOutput, result, new ScheduleComparers());
        }

        #region - Helper Methods -

        internal static void CreateValidSchedules()
        {
            foreach (var scheduleMapping in CreateValidScheduleMappings())
            {
                ValidSchedules.Add(scheduleMapping.Key, scheduleMapping.Value);
            }
        }

        private static ICloudSchedule CreateCloudSchedule(
            string frequency,
            string? start,
            string? end,
            IList<IInterval>? intervals)
        {
            intervals ??= [];

            var freqDetails = new FrequencyDetails(ConvertStringToTimeOnly(start), ConvertStringToTimeOnly(end), intervals);

            var cloudSchedule = new CloudSchedule(frequency, freqDetails);
            return cloudSchedule;
        }

        private static IServerSchedule CreateServerSchedule(
            string frequency,
            string? start,
            string? end,
            IList<IInterval>? intervals)
        {
            intervals ??= [];

            var mockFreqDetails = new Mock<IFrequencyDetails>();
            mockFreqDetails.Setup(f => f.StartAt).Returns(ConvertStringToTimeOnly(start));
            mockFreqDetails.Setup(f => f.EndAt).Returns(ConvertStringToTimeOnly(end));
            mockFreqDetails.Setup(f => f.Intervals).Returns(intervals);

            var mockSchedule = new Mock<IServerSchedule>();
            mockSchedule.Setup(s => s.Frequency).Returns(frequency);
            mockSchedule.Setup(s => s.FrequencyDetails).Returns(mockFreqDetails.Object);
            return mockSchedule.Object;
        }

        private static TimeOnly? ConvertStringToTimeOnly(string? input)
            => input is null ? null : TimeOnly.Parse(input);

        internal static Dictionary<IServerSchedule, ICloudSchedule> CreateValidScheduleMappings() => new()
            {
                // Hourly

                // 1 - hourly on the 30, turns into hourly with all weekdays set
                {
                    CreateServerSchedule(ScheduleFrequencies.Hourly, "00:30:00", "23:30:00",
                    [
                        Interval.WithHours(1)
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Hourly, "00:30:00", "23:30:00",
                    [
                        Interval.WithHours(1),
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday),
                        Interval.WithWeekday(WeekDays.Saturday),
                        Interval.WithWeekday(WeekDays.Sunday)
                    ])
                },

                // 2 - Every 2 hours on the 30 turns into daily with all weekdays set
                {
                    CreateServerSchedule(ScheduleFrequencies.Hourly, "00:30:00", "00:30:00",
                    [
                        Interval.WithHours(2)
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Daily, "00:30:00", "00:30:00",
                    [
                        Interval.WithHours(2),
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday),
                        Interval.WithWeekday(WeekDays.Saturday),
                        Interval.WithWeekday(WeekDays.Sunday)
                    ])
                },

                // 3 - Every hour on :15 turns into hourly with all weekdays set
                {
                    CreateServerSchedule(ScheduleFrequencies.Hourly, "00:15:00", "23:15:00",
                    [
                        Interval.WithHours(1)
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Hourly, "00:15:00", "23:15:00",
                    [
                        Interval.WithHours(1),
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday),
                        Interval.WithWeekday(WeekDays.Saturday),
                        Interval.WithWeekday(WeekDays.Sunday)
                    ])
                },

                // 4 - Hourly every .5 hours on :00, :30 which can't be done. Turns into hourly every 60 minutes
                {
                    CreateServerSchedule(ScheduleFrequencies.Hourly, "00:00:00", "00:00:00",
                    [
                        Interval.WithMinutes(30)
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Hourly, "00:00:00", "00:00:00",
                    [
                        Interval.WithMinutes(60),
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday),
                        Interval.WithWeekday(WeekDays.Saturday),
                        Interval.WithWeekday(WeekDays.Sunday)
                    ])
                },

                // 5 - Hourly every 4 hours, turns into daily
                {
                    CreateServerSchedule(ScheduleFrequencies.Hourly, "03:45:00", "23:45:00",
                    [
                        Interval.WithHours(4)
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Daily, "03:45:00", "23:45:00",
                    [
                        Interval.WithHours(4),
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday),
                        Interval.WithWeekday(WeekDays.Saturday),
                        Interval.WithWeekday(WeekDays.Sunday)
                    ])
                },


                // Daily
                // Daily Server Schedule can not have any intervals. They are all trimmed by the RestAPI

                // 6 - Daily requires weekday intervals
                {
                    CreateServerSchedule(ScheduleFrequencies.Daily, "02:00:00", null, null),
                    CreateCloudSchedule(ScheduleFrequencies.Daily, "02:00:00", null,
                    [
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday),
                        Interval.WithWeekday(WeekDays.Saturday),
                        Interval.WithWeekday(WeekDays.Sunday)
                    ])
                },

                // 7 - Daily on the :30. End time must be removed from cloud because no hours intervals exist
                {
                    CreateServerSchedule(ScheduleFrequencies.Daily, "23:30:00", "00:30:00", null),
                    CreateCloudSchedule(ScheduleFrequencies.Daily, "23:30:00", null,
                    [
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday),
                        Interval.WithWeekday(WeekDays.Saturday),
                        Interval.WithWeekday(WeekDays.Sunday)
                    ])
                },

                // 8 - Daily on the :15. End time must be removed from cloud because no hours intervals exist
                {
                    CreateServerSchedule(ScheduleFrequencies.Daily, "12:15:00", "00:15:00", null),
                    CreateCloudSchedule(ScheduleFrequencies.Daily, "12:15:00", null,
                    [
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday),
                        Interval.WithWeekday(WeekDays.Saturday),
                        Interval.WithWeekday(WeekDays.Sunday)
                    ])
                },

                // Weekly
                // Weekly Server Schedule can not have end time. It is removed by the RestAPI

                // 9 - Weekly with more than 1 weekday is not allowed, must be daily
                {
                    CreateServerSchedule(ScheduleFrequencies.Weekly, "06:00:00", null,
                    [
                        Interval.WithWeekday(WeekDays.Monday),
                        Interval.WithWeekday(WeekDays.Tuesday),
                        Interval.WithWeekday(WeekDays.Wednesday),
                        Interval.WithWeekday(WeekDays.Thursday),
                        Interval.WithWeekday(WeekDays.Friday)
                    ]),
                    // This is what I expected base on docs
                    // But Daily can't have end time, without having a hours interval
                    //CreateCloudSchedule(ScheduleFrequencies.Daily, "06:00:00", "06:00:00",
                    //[
                    //    Interval.WithWeekday(WeekDays.Monday),
                    //    Interval.WithWeekday(WeekDays.Tuesday),
                    //    Interval.WithWeekday(WeekDays.Wednesday),
                    //    Interval.WithWeekday(WeekDays.Thursday),
                    //    Interval.WithWeekday(WeekDays.Friday),
                    //    Interval.WithHours(24)
                    //])
                    CreateCloudSchedule(ScheduleFrequencies.Daily, "06:00:00", null,
                    [Interval.WithWeekday(WeekDays.Monday),Interval.WithWeekday(WeekDays.Tuesday),Interval.WithWeekday(WeekDays.Wednesday),Interval.WithWeekday(WeekDays.Thursday),Interval.WithWeekday(WeekDays.Friday)])
                },

                // 10 - Weekly on Saturday only
                {
                    CreateServerSchedule(ScheduleFrequencies.Weekly, "23:00:00", null,
                    [
                        Interval.WithWeekday(WeekDays.Saturday)
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Weekly, "23:00:00", null,
                    [
                        Interval.WithWeekday(WeekDays.Saturday)
                    ])
                },


                // Monthly
                // Monthly Server Schedule can not have end time. It is removed by the RestAPI

                // 11 - Last of the month
                {
                    CreateServerSchedule(ScheduleFrequencies.Monthly, "23:00:00", null,
                    [
                        Interval.WithMonthDay("1")
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Monthly, "23:00:00", null,
                    [
                        Interval.WithMonthDay("1")
                    ])
                },

                // 12 - First day of month
                {
                    CreateServerSchedule(ScheduleFrequencies.Monthly, "01:30:00", null,
                    [
                        Interval.WithMonthDay("1")
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Monthly, "01:30:00", null,
                    [
                        Interval.WithMonthDay("1")
                    ])
                },

                // 13 - Multiple days
                {
                    CreateServerSchedule(ScheduleFrequencies.Monthly, "13:55:00", null,
                    [
                        Interval.WithMonthDay("1"),
                        Interval.WithMonthDay("2"),
                        Interval.WithMonthDay("3")
                    ]),
                    CreateCloudSchedule(ScheduleFrequencies.Monthly, "13:55:00", null,
                    [
                        Interval.WithMonthDay("1"),
                        Interval.WithMonthDay("2"),
                        Interval.WithMonthDay("3")
                    ])
                }
            };

        #endregion
    }
}
