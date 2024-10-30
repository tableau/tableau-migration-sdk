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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Schedules
{
    public class ServerToCloudExtractRefreshTaskConverterTests
    {
        private readonly ServerToCloudExtractRefreshTaskConverter _converter;
        private readonly Mock<ILogger<ServerScheduleValidator>> _mockServerScheduleValidatorLogger;
        private readonly Mock<ILogger<CloudScheduleValidator>> _mockCloudScheduleValidatorLogger;
        private readonly Mock<ILogger<ServerToCloudExtractRefreshTaskConverter>> _mockConverterLogger;
        private readonly ISharedResourcesLocalizer _localizer;

        //private static readonly List<Tuple<IServerExtractRefreshTask, ICloudExtractRefreshTask>> ValidTasks = new List<Tuple<IServerExtractRefreshTask, ICloudExtractRefreshTask>>();
        public static TheoryData<IServerExtractRefreshTask, ICloudExtractRefreshTask> ValidTasks = new();

        /// <summary>
        /// Static constructor to create the test data
        /// </summary>
        static ServerToCloudExtractRefreshTaskConverterTests()
        {
            CreateValidExtractTasks();
        }

        /// <summary>
        /// Non-Static constructor to build the test object
        /// </summary>
        public ServerToCloudExtractRefreshTaskConverterTests()
        {
            // Create the real localizer
            var services = new ServiceCollection();
            services.AddTableauMigrationSdk();
            var container = services.BuildServiceProvider();

            _mockServerScheduleValidatorLogger = new Mock<ILogger<ServerScheduleValidator>>();
            _mockCloudScheduleValidatorLogger = new Mock<ILogger<CloudScheduleValidator>>();
            _mockConverterLogger = new Mock<ILogger<ServerToCloudExtractRefreshTaskConverter>>();
            _localizer = container.GetRequiredService<ISharedResourcesLocalizer>();

            var serverValidator = new ServerScheduleValidator(_mockServerScheduleValidatorLogger.Object, _localizer);
            var cloudValidator = new CloudScheduleValidator(_mockCloudScheduleValidatorLogger.Object, _localizer);
            _converter = new ServerToCloudExtractRefreshTaskConverter(serverValidator, cloudValidator, _mockConverterLogger.Object, _localizer);
        }




        [Theory]
        [MemberData(nameof(ValidTasks))]
        public void ValidSchedule(IServerExtractRefreshTask input, ICloudExtractRefreshTask expectedCloudExtractTask)
        {
            // Act
            var result = _converter.Convert(input);

            // Assert
            Assert.Equal(expectedCloudExtractTask.Schedule, result.Schedule, new ScheduleComparers());
        }

        #region - Helper methods - 
        private static void CreateValidExtractTasks()
        {
            // Hourly

            // 1 - hourly on the 30, turns into hourly with all weekdays set
            ValidTasks.Add(
                CreateServerExtractTask("Hourly", "00:30:00", "23:30:00",
                [
                    Interval.WithHours(1)
                ]),
                CreateCloudExtractTask("Hourly", "00:30:00", "23:30:00",
                [
                    Interval.WithHours(1),
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                    Interval.WithWeekday("Saturday"),
                    Interval.WithWeekday("Sunday")
                ])
            );

            // 2 - Every 2 hours on the 30 turns into daily with all weekdays set
            ValidTasks.Add(
                CreateServerExtractTask("Hourly", "00:30:00", "00:30:00",
                [
                    Interval.WithHours(2)
                ]),
                CreateCloudExtractTask("Daily", "00:30:00", "00:30:00",
                [
                    Interval.WithHours(2),
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                    Interval.WithWeekday("Saturday"),
                    Interval.WithWeekday("Sunday")
                ])
            );

            // 3 - Every hour on :15 turns into hourly with all weekdays set
            ValidTasks.Add(
                CreateServerExtractTask("Hourly", "00:15:00", "23:15:00",
                [
                    Interval.WithHours(1)
                ]),
                CreateCloudExtractTask("Hourly", "00:15:00", "23:15:00",
                [
                    Interval.WithHours(1),
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                    Interval.WithWeekday("Saturday"),
                    Interval.WithWeekday("Sunday")
                ])
            );

            // 4 - Hourly every .5 hours on :00, :30 which can't be done. Turns into hourly every 60 minutes
            ValidTasks.Add(
                CreateServerExtractTask("Hourly", "00:00:00", "00:00:00",
                [
                    Interval.WithMinutes(30)
                ]),
                CreateCloudExtractTask("Hourly", "00:00:00", "00:00:00",
                [
                    Interval.WithMinutes(60),
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                    Interval.WithWeekday("Saturday"),
                    Interval.WithWeekday("Sunday")
                ])
            );

            // 5 - Hourly every 4 hours, turns into daily
            ValidTasks.Add(
                CreateServerExtractTask("Hourly", "03:45:00", "23:45:00",
                [
                    Interval.WithHours(4)
                ]),
                CreateCloudExtractTask("Daily", "03:45:00", "23:45:00",
                [
                    Interval.WithHours(4),
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                    Interval.WithWeekday("Saturday"),
                    Interval.WithWeekday("Sunday")
                ])
            );


            // Daily
            // Daily Server Schedule can not have any intervals. They are all trimmed by the RestAPI

            // 6 - Daily requires weekday intervals
            ValidTasks.Add(
                CreateServerExtractTask("Daily", "02:00:00", null, null),
                CreateCloudExtractTask("Daily", "02:00:00", null,
                [
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                    Interval.WithWeekday("Saturday"),
                    Interval.WithWeekday("Sunday")
                ])
            );

            // 7 - Daily on the :30. End time must be removed from cloud because no hours intervals exist
            ValidTasks.Add(
                CreateServerExtractTask("Daily", "23:30:00", "00:30:00", null),
                CreateCloudExtractTask("Daily", "23:30:00", null,
                [
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                    Interval.WithWeekday("Saturday"),
                    Interval.WithWeekday("Sunday")
                ])
            );

            // 8 - Daily on the :15. End time must be removed from cloud because no hours intervals exist
            ValidTasks.Add(
                CreateServerExtractTask("Daily", "12:15:00", "00:15:00", null),
                CreateCloudExtractTask("Daily", "12:15:00", null,
                [
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                    Interval.WithWeekday("Saturday"),
                    Interval.WithWeekday("Sunday")
                ])
            );

            // Weekly
            // Weekly Server Schedule can not have end time. It is removed by the RestAPI

            // 9 - Weekly with more than 1 weekday is not allowed, must be daily
            ValidTasks.Add(
                CreateServerExtractTask("Weekly", "06:00:00", null,
                [
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                ]),
                // This is what I expected base on docs
                // But Daily can't have end time, without having a hours interval
                //CreateCloudExtractTask("Daily", "06:00:00", "06:00:00",
                //[
                //    Interval.WithWeekday("Monday"),
                //    Interval.WithWeekday("Tuesday"),
                //    Interval.WithWeekday("Wednesday"),
                //    Interval.WithWeekday("Thursday"),
                //    Interval.WithWeekday("Friday"),
                //    Interval.WithHours(24)
                //])
                CreateCloudExtractTask("Daily", "06:00:00", null,
                [
                    Interval.WithWeekday("Monday"),
                    Interval.WithWeekday("Tuesday"),
                    Interval.WithWeekday("Wednesday"),
                    Interval.WithWeekday("Thursday"),
                    Interval.WithWeekday("Friday"),
                ])
            );

            // 10 - Weekly on Saturday only
            ValidTasks.Add(
                CreateServerExtractTask("Weekly", "23:00:00", null,
                [
                    Interval.WithWeekday("Saturday")
                ]),
                CreateCloudExtractTask("Weekly", "23:00:00", null,
                [
                    Interval.WithWeekday("Saturday")
                ])
            );


            // Monthly
            // Monthly Server Schedule can not have end time. It is removed by the RestAPI

            // 11 - Last of the month
            ValidTasks.Add(
                CreateServerExtractTask("Monthly", "23:00:00", null,
                [
                    Interval.WithMonthDay("1")
                ]),
                CreateCloudExtractTask("Monthly", "23:00:00", null,
                [
                    Interval.WithMonthDay("1")
                ])
            );

            // 12 - First day of month
            ValidTasks.Add(
                CreateServerExtractTask("Monthly", "01:30:00", null,
                [
                    Interval.WithMonthDay("1")
                ]),
                CreateCloudExtractTask("Monthly", "01:30:00", null,
                [
                    Interval.WithMonthDay("1")
                ])
            );

            // 13 - Multiple days
            ValidTasks.Add(
                CreateServerExtractTask("Monthly", "13:55:00", null,
                [
                    Interval.WithMonthDay("1"),
                    Interval.WithMonthDay("2"),
                    Interval.WithMonthDay("3")
                ]),
                CreateCloudExtractTask("Monthly", "13:55:00", null,
                [
                    Interval.WithMonthDay("1"),
                    Interval.WithMonthDay("2"),
                    Interval.WithMonthDay("3")
                ])
            );


        }

        private static IServerExtractRefreshTask CreateServerExtractTask(string frequency, string? start, string? end, IList<IInterval>? intervals)
        {
            if (intervals is null)
            {
                intervals = new List<IInterval>();
            }

            var mockFreqDetails = new Mock<IFrequencyDetails>();
            mockFreqDetails.Setup(f => f.StartAt).Returns(ConvertStringToTimeOnly(start));
            mockFreqDetails.Setup(f => f.EndAt).Returns(ConvertStringToTimeOnly(end));
            mockFreqDetails.Setup(f => f.Intervals).Returns(intervals);

            var mockSchedule = new Mock<IServerSchedule>();
            mockSchedule.Setup(s => s.Frequency).Returns(frequency);
            mockSchedule.Setup(s => s.FrequencyDetails).Returns(mockFreqDetails.Object);

            var mockTask = new Mock<IServerExtractRefreshTask>();
            mockTask.Setup(t => t.Schedule).Returns((IServerSchedule)mockSchedule.Object);

            //return new ServerExtractRefreshTask(mockTask.Object.Id, mockTask.Object.Type, mockTask.Object.ContentType, mockTask.Object.Content, mockTask.Object.Schedule);

            return mockTask.Object;
        }

        private static ICloudExtractRefreshTask CreateCloudExtractTask(string frequency, string? start, string? end, IList<IInterval>? intervals)
        {
            if (intervals is null)
            {
                intervals = new List<IInterval>();
            }

            //var mockFreqDetails = new Mock<IFrequencyDetails>();
            //mockFreqDetails.Setup(f => f.StartAt).Returns(ConvertStringToTimeOnly(start));
            //mockFreqDetails.Setup(f => f.EndAt).Returns(ConvertStringToTimeOnly(end));
            //mockFreqDetails.Setup(f => f.Intervals).Returns(intervals);

            var freqDetails = new FrequencyDetails(ConvertStringToTimeOnly(start), ConvertStringToTimeOnly(end), intervals);

            //var mockSchedule = new Mock<ICloudSchedule>();
            //mockSchedule.Setup(s => s.Frequency).Returns(frequency);
            //mockSchedule.Setup(s => s.FrequencyDetails).Returns(mockFreqDetails.Object);

            var cloudSchedule = new CloudSchedule(frequency, freqDetails);

            var mockTask = new Mock<ICloudExtractRefreshTask>();
            mockTask.Setup(t => t.Schedule).Returns(cloudSchedule);

            // Returning concrete object so the ToString message works for debugging purposes
            //return new CloudExtractRefreshTask(mockTask.Object.Id, mockTask.Object.Type, mockTask.Object.ContentType, mockTask.Object.Content, mockTask.Object.Schedule);

            return mockTask.Object;
        }

        private static TimeOnly? ConvertStringToTimeOnly(string? input)
        {
            if (input is null)
            {
                return null;
            }

            return TimeOnly.Parse(input);
        }
        #endregion
    }

}