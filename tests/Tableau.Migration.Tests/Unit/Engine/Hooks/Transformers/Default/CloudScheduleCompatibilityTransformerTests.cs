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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class CloudScheduleCompatibilityTransformerTests : AutoFixtureTestBase
    {
        protected readonly TestLogger<CloudScheduleCompatibilityTransformer<ICloudExtractRefreshTask>> Logger = new();
        protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();
        protected readonly CloudScheduleCompatibilityTransformer<ICloudExtractRefreshTask> Transformer;

        public CloudScheduleCompatibilityTransformerTests()
        {
            Transformer = new(MockSharedResourcesLocalizer.Object, Logger);
        }

        [Theory]
        [InlineData(ScheduleFrequencies.Monthly)]
        [InlineData(ScheduleFrequencies.Daily)]
        public async Task Skips_intervals_longer_than_1_hour(string frequency)
        {
            // Arrange            
            var input = Create<ICloudExtractRefreshTask>();
            input.Type = ExtractRefreshType.FullRefresh;
            input.Schedule.Frequency = frequency;

            // Act
            var result = await Transformer.ExecuteAsync(input, Cancel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(input.Schedule.FrequencyDetails.Intervals.Count, result.Schedule.FrequencyDetails.Intervals.Count);
            Assert.Empty(Logger.Messages.Where(m => m.LogLevel == LogLevel.Warning).ToList());
        }

        [Fact]
        public async Task Transforms_intervals_shorter_than_1_hour()
        {
            // Arrange            
            var input = Create<ICloudExtractRefreshTask>();
            input.Type = ExtractRefreshType.FullRefresh;
            input.Schedule.FrequencyDetails.Intervals = ImmutableList.Create(Interval.WithMinutes(15));
            input.Schedule.Frequency = ScheduleFrequencies.Hourly;
            
            // Act
            var result = await Transformer.ExecuteAsync(input, Cancel);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Schedule.FrequencyDetails.Intervals);
            Assert.Equal(Interval.WithHours(1), result.Schedule.FrequencyDetails.Intervals[0]);
            Assert.Single(Logger.Messages.Where(m => m.LogLevel == LogLevel.Warning));
        }

        [Fact]
        public async Task Transforms_weekly_intervals_with_multiple_weekdays()
        {
            // Arrange            
            var input = Create<ICloudExtractRefreshTask>();
            input.Type = ExtractRefreshType.FullRefresh;
            input.Schedule.FrequencyDetails.Intervals = ImmutableList.Create(
                Interval.WithWeekday(WeekDays.Sunday),
                Interval.WithWeekday(WeekDays.Monday));
            input.Schedule.Frequency = ScheduleFrequencies.Weekly;
            
            // Act
            var result = await Transformer.ExecuteAsync(input, Cancel);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Schedule.FrequencyDetails.Intervals);
            Assert.Equal(Interval.WithWeekday(WeekDays.Sunday), result.Schedule.FrequencyDetails.Intervals[0]);
            Assert.Single(Logger.Messages.Where(m => m.LogLevel == LogLevel.Warning));
        }
    }
}
