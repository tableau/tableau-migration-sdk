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

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.ContentConverters.Schedules;
using Tableau.Migration.Engine.Conversion.Schedules;
using Tableau.Migration.Tests.Unit.Engine.Conversion.Schedules;
using Xunit;

namespace Tableau.Migration.Tests.Unit.ContentConverters.Schedules
{
    public class ServerToCloudExtractRefreshTaskConverterTests : AutoFixtureTestBase
    {
        private readonly ServerToCloudExtractRefreshTaskConverter _converter;
        private readonly Mock<IScheduleConverter<IServerSchedule, ICloudSchedule>> _mockScheduleConverter = new();

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

            _converter = new ServerToCloudExtractRefreshTaskConverter(_mockScheduleConverter.Object);
        }

        [Theory]
        [MemberData(nameof(ValidTasks))]
        public async Task ValidConversionAsync(IServerExtractRefreshTask input, ICloudExtractRefreshTask expectedCloudExtractTask)
        {
            // Setup
            _mockScheduleConverter.Setup(c => c.ConvertAsync(input.Schedule, Cancel)).ReturnsAsync(expectedCloudExtractTask.Schedule);

            // Act            
            var result = await _converter.ConvertAsync(input, Cancel);

            // Assert            
            Assert.Equal(expectedCloudExtractTask.Type, result.Type);
            Assert.Equal(expectedCloudExtractTask.ContentType, result.ContentType);
            Assert.Equal(expectedCloudExtractTask.Content, result.Content);
            Assert.Equal(expectedCloudExtractTask.Schedule, result.Schedule, new ScheduleComparers());
        }

        #region - Helper Methods - 

        private static void CreateValidExtractTasks()
        {
            foreach (var scheduleMapping in ServerToCloudScheduleConverterTests.CreateValidScheduleMappings())
            {
                ValidTasks.Add(CreateServerExtractTask(scheduleMapping.Key), CreateCloudExtractTask(scheduleMapping.Value));
            }
        }

        private static IServerExtractRefreshTask CreateServerExtractTask(IServerSchedule schedule)
        {
            var mockTask = new Mock<IServerExtractRefreshTask>();
            mockTask.Setup(t => t.Schedule).Returns(schedule);

            return mockTask.Object;
        }

        private static ICloudExtractRefreshTask CreateCloudExtractTask(ICloudSchedule schedule)
        {
            var mockTask = new Mock<ICloudExtractRefreshTask>();
            mockTask.Setup(t => t.Schedule).Returns(schedule);

            return mockTask.Object;
        }

        #endregion
    }
}