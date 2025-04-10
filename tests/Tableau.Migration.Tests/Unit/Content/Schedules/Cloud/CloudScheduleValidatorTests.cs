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
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Schedules.Cloud
{
    public class CloudScheduleValidatorTests
    {
        protected const string TIME_12_00_00 = "12:00:00";
        protected const string TIME_13_00_00 = "13:00:00";
        protected const string TIME_13_30_00 = "13:30:00";
        protected const string TIME_12_00 = "12:00";
        protected const string TIME_13_00 = "13:00";
        protected const string WEEKDAY_INVALID = "Thanksgiving";

        private readonly Mock<ILogger<CloudScheduleValidator>> _loggerMock;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly CloudScheduleValidator _validator;

        public CloudScheduleValidatorTests()
        {
            // Create the real localizer
            var services = new ServiceCollection();
            services.AddTableauMigrationSdk();
            var container = services.BuildServiceProvider();

            _loggerMock = new Mock<ILogger<CloudScheduleValidator>>();
            _localizer = container.GetRequiredService<ISharedResourcesLocalizer>();
            _validator = new CloudScheduleValidator(_loggerMock.Object, _localizer);
        }

        private Mock<ICloudSchedule> CreateMockSchedule(string frequency, string? start, string? end, List<IInterval> intervals)
        {
            var schedule = new Mock<ICloudSchedule>();
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

        public class HourlyTests : CloudScheduleValidatorTests
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

                // Act & Assert
                var schedule = CreateMockSchedule(frequency, start: TIME_12_00, end: TIME_13_00, intervals);
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

                var schedule = CreateMockSchedule(frequency, start: null, end: TIME_13_00, new List<IInterval> { new Mock<IInterval>().Object });

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

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: null, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveEndAtTimeError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidStartEndDifference()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_30_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.StartEndTimeDifferenceError], exception.Message);
            }

            [Fact]
            public void Validate_HoursAndMinutesSet()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithMinutes(60),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ExactlyOneHourOrMinutesError], exception.Message);
            }

            [Fact]
            public void Validate_InvalidHourSet()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(2),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.IntervalMustBe1HourOr60MinError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_MinutesSet()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithMinutes(30),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.IntervalMustBe1HourOr60MinError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_NoWeekdaySet()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.AtLeastOneValidWeekdayError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidWeekday()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithWeekday(WEEKDAY_INVALID)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.InvalidWeekdayError], exception.Message);
            }
        }

        public class DailyTests : CloudScheduleValidatorTests
        {
            private readonly string frequency = ScheduleFrequencies.Daily;

            [Fact]
            public void Validate_DoesNotThrow()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(2),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                // Act & Assert
                var schedule = CreateMockSchedule(frequency, start: TIME_12_00, end: TIME_13_00, intervals);
                _validator.Validate(schedule.Object);
            }

            [Fact]
            public void Validate_MissingStart()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(2),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: null, end: TIME_13_00, new List<IInterval> { new Mock<IInterval>().Object });

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveStartAtTimeError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_NoWeekdaySet()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(2),
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.AtLeastOneValidWeekdayError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidWeekday()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(2),
                    Interval.WithWeekday(WEEKDAY_INVALID)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.InvalidWeekdayError], exception.Message);
            }

            [Fact]
            public void Validate_InvalidHour()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(3),
                    Interval.WithWeekday(WeekDays.Tuesday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.InvalidHourlyIntervalForCloudError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_MissingEnd()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(2),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: null, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveEndAtTimeError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_MissingEnd_With24Hours()
            {
                // Note: The docs say if the hours are less then 24, then end is not required
                // Testing on Tableau Cloud has shown that to be false. If hours are set, end is required.

                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(24),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: null, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveEndAtTimeError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidStartEndDifference()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(2),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_30_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.StartEndTimeDifferenceError], exception.Message);
            }
        }

        public class WeeklyTests : CloudScheduleValidatorTests
        {
            private readonly string frequency = ScheduleFrequencies.Weekly;

            [Fact]
            public void Validate_DoesNotThrow()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithWeekday(WeekDays.Wednesday)
                };

                // Act & Assert
                var schedule = CreateMockSchedule(frequency, start: TIME_12_00, end: null, intervals);
                _validator.Validate(schedule.Object);
            }

            [Fact]
            public void Validate_InvalidWeekdayCount()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithWeekday(WeekDays.Monday),
                    Interval.WithWeekday(WeekDays.Wednesday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: null, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustHaveExactlyOneWeekdayIntervalError, frequency], exception.Message);
            }

            [Fact]
            public void Validate_InvalidWeekday()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithWeekday(WEEKDAY_INVALID)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: null, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.InvalidWeekdayError], exception.Message);
            }

            [Fact]
            public void Validate_HasEndTime()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                var schedule = CreateMockSchedule(frequency, start: TIME_12_00_00, end: TIME_13_00_00, intervals);

                // Act & Assert
                var exception = Assert.Throws<InvalidScheduleException>(() => _validator.Validate(schedule.Object));
                Assert.Equal(_localizer[SharedResourceKeys.ScheduleMustNotHaveEndAtTimeError, frequency], exception.Message);
            }
        }
    }
}
