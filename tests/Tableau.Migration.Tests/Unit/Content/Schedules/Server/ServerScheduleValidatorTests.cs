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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Content.Schedules.Server
{
    public class ServerScheduleValidatorTests
    {
        private readonly Mock<ILogger<ServerScheduleValidator>> _loggerMock;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ServerScheduleValidator _validator;

        public ServerScheduleValidatorTests()
        {
            // Create the real localizer
            var services = new ServiceCollection();
            services.AddTableauMigrationSdk();
            var container = services.BuildServiceProvider();

            _loggerMock = new Mock<ILogger<ServerScheduleValidator>>();
            _localizer = container.GetRequiredService<ISharedResourcesLocalizer>();
            _validator = new ServerScheduleValidator(_loggerMock.Object, _localizer);
        }

        private Mock<IServerSchedule> CreateMockSchedule(string frequency, string? start, string? end, List<IInterval> intervals)
        {
            var schedule = new Mock<IServerSchedule>();
            var frequencyDetails = new Mock<IFrequencyDetails>();

            schedule.Setup(s => s.Frequency).Returns(frequency);

            if (start is not null)
                frequencyDetails.Setup(f => f.StartAt).Returns(TimeOnly.Parse(start));
            if (end is not null)
                frequencyDetails.Setup(f => f.EndAt).Returns(TimeOnly.Parse(end));


            frequencyDetails.Setup(f => f.Intervals).Returns(intervals);

            schedule.Setup(s => s.FrequencyDetails).Returns(frequencyDetails.Object);
            return schedule;
        }

        public class HourlyTests : ServerScheduleValidatorTests
        {
            private readonly string frequency = ScheduleFrequencies.Hourly;

            [Fact]
            public void Validate_DoesNotThrow()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: "13:00:00", intervals);

                // Act & Assert
                _validator.Validate(schedule.Object);
            }

            [Fact]
            public void Validate_MissingStart()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: null, end: "13:00:00", intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveStartAtTimeError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_MissingEnd()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: null, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveEndAtTimeError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_NoIntervals()
            {
                // Arrange
                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: "13:00:00", new List<IInterval>());

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.AtLeastOneIntervalWithHourOrMinutesError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidInterval()
            {
                // Arrange
                var badInterval = new Mock<IInterval>();
                badInterval.Setup(i => i.WeekDay).Returns((string?)null);

                //var intervals = new List<IInterval>()
                //{
                //    badInterval.Object
                //};

                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: "13:00:00", new List<IInterval> { badInterval.Object });

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.AtLeastOneIntervalWithHourOrMinutesError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidHour()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(3),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: "13:00:00", intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.InvalidHourlyIntervalForServerError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidDay()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithWeekday("Thanksgiving")
                };

                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: "13:00:00", intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.AtLeastOneIntervalWithHourOrMinutesError, frequency], exception.Message);
            }
        }

        public class DailyTests : ServerScheduleValidatorTests
        {
            private readonly string frequency = ScheduleFrequencies.Daily;

            [Fact]
            public void Validate_DoesNotThrow()
            {
                // Arrange
                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: null, new List<IInterval>());

                // Act & Assert
                _validator.Validate(schedule.Object);
            }

            [Fact]
            public void Validate_MissingStart()
            {
                // Arrange
                var schedule = CreateMockSchedule(frequency, start: null, end: null, new List<IInterval>());

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveStartAtTimeError, frequency], exception.Message);
            }
        }

        public class WeeklyTests : ServerScheduleValidatorTests
        {
            private readonly string frequency = ScheduleFrequencies.Weekly;

            [Fact]
            public void Validate_DoesNotThrow()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: null, intervals);

                // Act & Assert
                _validator.Validate(schedule.Object);
            }

            [Fact]
            public void Validate_MissingStart()
            {
                // Arrange
                var schedule = CreateMockSchedule(frequency, start: null, end: null, new List<IInterval>());

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveStartAtTimeError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_NoInterval()
            {
                // Arrange
                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: null, new List<IInterval>());

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.WeeklyScheduleIntervalError], exception.Message);
            }
        }

        public class MonthlyWeeklyTests : ServerScheduleValidatorTests
        {
            private readonly string frequency = ScheduleFrequencies.Monthly;

            [Fact]
            public void Validate_DoesNotThrow()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithMonthDay("1")
                };

                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: null, intervals);

                // Act & Assert
                _validator.Validate(schedule.Object);
            }

            [Fact]
            public void Validate_MissingStart()
            {
                // Arrange
                var schedule = CreateMockSchedule(frequency, start: null, end: null, new List<IInterval>());

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveStartAtTimeError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidMonthday()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithMonthDay("Thankgiving")
                };

                var schedule = CreateMockSchedule(frequency, start: "12:00:00", end: null, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.InvalidMonthDayError], exception.Message);
            }

        }
    }
}
