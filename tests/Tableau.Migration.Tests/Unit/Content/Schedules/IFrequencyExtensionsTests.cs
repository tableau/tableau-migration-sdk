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

using System.Collections.Immutable;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Schedules
{
    public class IFrequencyExtensionsTests
    {
        public class IsCloudCompatible
        {
            [Theory]
            [InlineData(ScheduleFrequencies.Hourly, 15, null, null, null, false)]
            [InlineData(ScheduleFrequencies.Hourly, 30, null, null, null, false)]
            [InlineData(ScheduleFrequencies.Hourly, 60, null, null, null, true)]
            [InlineData(ScheduleFrequencies.Hourly, null, 1, null, null, true)]
            [InlineData(ScheduleFrequencies.Daily, null, 2, null, null, true)]
            [InlineData(ScheduleFrequencies.Hourly, null, null, WeekDays.Monday, null, true)]
            [InlineData(ScheduleFrequencies.Hourly, null, null, null, "26", true)]
            public void Parses(
                string frequency,
                int? minutes, 
                int? hours, 
                string? weekday, 
                string? monthDay, 
                bool expectedResult)
            {
                // Arrange 
                var testIntervals = ImmutableList.Create(new Interval(hours, minutes, weekday, monthDay) as IInterval);

                // Act/Assert
                Assert.Equal(expectedResult, frequency.IsCloudCompatible(testIntervals));
            }
        }

        public class ToCloudCompatible
        {
            [Theory]
            [InlineData(ScheduleFrequencies.Hourly, 15)]
            [InlineData(ScheduleFrequencies.Hourly, 30)]
            public void Converts_unsupported(
                string frequency, 
                int? minutes)
            {
                // Arrange 
                var testIntervals = ImmutableList.Create(new Interval(null, minutes, null, null) as IInterval);
                
                // Act
                var cloudInterval = frequency.ToCloudCompatible(testIntervals);

                // Assert
                Assert.True(frequency.IsCloudCompatible(cloudInterval));
                Assert.Single(cloudInterval);
                Assert.Equal(Interval.WithHours(1), cloudInterval[0]);
            }

            [Theory]
            [InlineData(ScheduleFrequencies.Daily, null, 2, null, null)]
            [InlineData(ScheduleFrequencies.Daily, null, 4, null, null)]
            [InlineData(ScheduleFrequencies.Weekly, null, null, "Monday", null)]
            [InlineData(ScheduleFrequencies.Monthly, null, null, null, "26")]
            public void Does_not_convert_supported(
                string frequency,
                int? minutes, 
                int? hours, 
                string? weekday, 
                string? monthDay)
            {
                // Arrange 
                var testInterval = ImmutableList.Create(new Interval(hours, minutes, weekday, monthDay) as IInterval);
                
                // Act
                var cloudInterval = frequency.ToCloudCompatible(testInterval);

                // Assert
                Assert.True(frequency.IsCloudCompatible(cloudInterval));
                Assert.Single(cloudInterval);
                Assert.Equal(testInterval[0], cloudInterval[0]);
            }
        }
    }
}
