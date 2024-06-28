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
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Schedules
{
    public class ILoggerExtensionsTests
    {
        private static readonly string LogMessage = $"Guid: {{0}}{Environment.NewLine}Original:{Environment.NewLine}{{1}}{Environment.NewLine}Updated:{Environment.NewLine}{{2}}";

        public abstract class ILoggerExtensionsTest : AutoFixtureTestBase
        {
            protected readonly TestLogger Logger = new();
        }

        public class LogIntervalsChanges : ILoggerExtensionsTest
        {
            [Fact]
            public void Logs_changes_when_intervals_differ()
            {
                // Arrange
                var originalIntervals = new List<IInterval>
                {
                    Interval.WithHours(1),
                    Interval.WithMinutes(1),
                    Interval.WithWeekday(WeekDays.Monday),
                    Interval.WithWeekday(WeekDays.Tuesday)
                }.ToImmutableList();
                var newIntervals = new List<IInterval>
                {
                    Interval.WithHours(2),
                    Interval.WithMinutes(15),
                    Interval.WithMonthDay("24")
                }.ToImmutableList();

                // Act
                var result = Logger.LogIntervalsChanges(
                    LogMessage,
                    Guid.NewGuid(),
                    originalIntervals,
                    newIntervals);

                // Assert
                Assert.True(result);

                var message = Assert.Single(Logger.Messages);
                Assert.Equal(LogLevel.Warning, message.LogLevel);
            }

            [Fact]
            public void Does_not_log_changes_when_intervals_same()
            {
                // Arrange
                var intervals = new List<IInterval>()
                {
                    Interval.WithHours(1),
                    Interval.WithMinutes(1),
                    Interval.WithWeekday(WeekDays.Monday)
                };

                // Act
                var result = Logger.LogIntervalsChanges(
                    LogMessage,
                    Guid.NewGuid(),
                    intervals.ToImmutableList(),
                    intervals.ToImmutableList());

                // Asserts
                Assert.False(result);
                Assert.Empty(Logger.Messages);
            }
        }
    }
}
