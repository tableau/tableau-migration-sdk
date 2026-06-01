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
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Conversion.FlowRunTasks;
using Tableau.Migration.Engine.Conversion.Schedules;
using Tableau.Migration.Tests.Unit.Engine.Conversion.Schedules;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Conversion.FlowRunTasks
{
    public class ServerToCloudFlowRunTaskConverterTests : AutoFixtureTestBase
    {
        private readonly ServerToCloudFlowRunTaskConverter _converter;
        private readonly Mock<IScheduleConverter<IServerSchedule, ICloudSchedule>> _mockScheduleConverter = new();

        public static TheoryData<IServerFlowRunTask, ICloudFlowRunTask> ValidTasks = new();

        /// <summary>
        /// Static constructor to create the test data
        /// </summary>
        static ServerToCloudFlowRunTaskConverterTests()
        {
            CreateValidFlowRunTasks();
        }

        /// <summary>
        /// Non-Static constructor to build the test object
        /// </summary>
        public ServerToCloudFlowRunTaskConverterTests()
        {
            // Create the real localizer
            var services = new ServiceCollection();
            services.AddTableauMigrationSdk();
            var container = services.BuildServiceProvider();

            _converter = new ServerToCloudFlowRunTaskConverter(_mockScheduleConverter.Object);
        }

        [Theory]
        [MemberData(nameof(ValidTasks))]
        public async Task ValidConversionAsync(IServerFlowRunTask input, ICloudFlowRunTask expectedCloudFlowRunTask)
        {
            // Setup
            _mockScheduleConverter.Setup(c => c.ConvertAsync(input.Schedule, Cancel)).ReturnsAsync(expectedCloudFlowRunTask.Schedule);

            // Act            
            var result = await _converter.ConvertAsync(input, Cancel);

            // Assert            
            Assert.Equal(expectedCloudFlowRunTask.Type, result.Type);
            Assert.Equal(expectedCloudFlowRunTask.Priority, result.Priority);
            Assert.Equal(expectedCloudFlowRunTask.ConsecutiveFailedCount, result.ConsecutiveFailedCount);
            Assert.Equal(expectedCloudFlowRunTask.Flow, result.Flow);
            Assert.Equal(expectedCloudFlowRunTask.Schedule, result.Schedule, new ScheduleComparers());
        }

        #region - Helper Methods - 

        private static void CreateValidFlowRunTasks()
        {
            foreach (var scheduleMapping in ServerToCloudScheduleConverterTests.CreateValidScheduleMappings())
            {
                // Create a shared Flow reference since the converter passes through source.Flow
                var flowReference = new Mock<IContentReference>().Object;
                ValidTasks.Add(CreateServerFlowRunTask(scheduleMapping.Key, flowReference), CreateCloudFlowRunTask(scheduleMapping.Value, flowReference));
            }
        }

        private static IServerFlowRunTask CreateServerFlowRunTask(IServerSchedule schedule, IContentReference flow)
        {
            var mockTask = new Mock<IServerFlowRunTask>();
            mockTask.Setup(t => t.Schedule).Returns(schedule);
            mockTask.Setup(t => t.Type).Returns("RunFlowTask");
            mockTask.Setup(t => t.Priority).Returns(50);
            mockTask.Setup(t => t.ConsecutiveFailedCount).Returns(0);
            mockTask.Setup(t => t.Flow).Returns(flow);

            return mockTask.Object;
        }

        private static ICloudFlowRunTask CreateCloudFlowRunTask(ICloudSchedule schedule, IContentReference flow)
        {
            var mockTask = new Mock<ICloudFlowRunTask>();
            mockTask.Setup(t => t.Schedule).Returns(schedule);
            mockTask.Setup(t => t.Type).Returns("RunFlowTask");
            mockTask.Setup(t => t.Priority).Returns(50);
            mockTask.Setup(t => t.ConsecutiveFailedCount).Returns(0);
            mockTask.Setup(t => t.Flow).Returns(flow);

            return mockTask.Object;
        }

        #endregion
    }
}
