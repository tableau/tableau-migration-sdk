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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Conversion.Schedules;
using Tableau.Migration.Engine.Conversion.Subscriptions;
using Tableau.Migration.Tests.Unit.Engine.Conversion.Schedules;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Conversion.Subscriptions
{
    public class ServerToCloudSubscriptionConverterTests : AutoFixtureTestBase
    {
        private readonly ServerToCloudSubscriptionConverter _converter;
        private readonly Mock<IScheduleConverter<IServerSchedule, ICloudSchedule>> _mockScheduleConverter = new();

        public static TheoryData<IServerSubscription, ICloudSubscription> ValidSubscriptions = new();

        /// <summary>
        /// Static constructor to create the test data
        /// </summary>
        static ServerToCloudSubscriptionConverterTests()
        {
            CreateValidSubscriptions();
        }

        /// <summary>
        /// Non-Static constructor to build the test object
        /// </summary>
        public ServerToCloudSubscriptionConverterTests()
        {
            // Create the real localizer
            var services = new ServiceCollection();
            services.AddTableauMigrationSdk();
            var container = services.BuildServiceProvider();

            _converter = new ServerToCloudSubscriptionConverter(_mockScheduleConverter.Object);
        }

        [Theory]
        [MemberData(nameof(ValidSubscriptions))]
        public async Task ValidConversionAsync(IServerSubscription input, ICloudSubscription expectedSubscription)
        {
            // Setup
            _mockScheduleConverter.Setup(c => c.ConvertAsync(input.Schedule, Cancel)).ReturnsAsync(expectedSubscription.Schedule);

            // Act            
            var result = await _converter.ConvertAsync(input, Cancel);

            // Assert           
            Assert.Equal(expectedSubscription.Content, result.Content);
            Assert.Equal(expectedSubscription.Schedule, result.Schedule, new ScheduleComparers());
        }

        #region - Helper Methods - 

        private static void CreateValidSubscriptions()
        {
            foreach (var scheduleMapping in ServerToCloudScheduleConverterTests.CreateValidScheduleMappings())
            {
                ValidSubscriptions.Add(CreateServerSubscription(scheduleMapping.Key), CreateCloudSubscription(scheduleMapping.Value));
            }
        }

        private static IServerSubscription CreateServerSubscription(IServerSchedule schedule)
        {
            var mockTask = new Mock<IServerSubscription>();
            mockTask.Setup(t => t.Schedule).Returns(schedule);
            mockTask.Setup(t => t.Id).Returns(Guid.NewGuid());
            mockTask.Setup(t => t.Subject).Returns($"{nameof(ICloudSubscription.Subject)}{Guid.NewGuid()}");

            return mockTask.Object;
        }

        private static ICloudSubscription CreateCloudSubscription(ICloudSchedule schedule)
        {
            var mockTask = new Mock<ICloudSubscription>();
            mockTask.Setup(t => t.Schedule).Returns(schedule);
            mockTask.Setup(t => t.Id).Returns(Guid.NewGuid());
            mockTask.Setup(t => t.Subject).Returns($"{nameof(ICloudSubscription.Subject)}{Guid.NewGuid()}");

            return mockTask.Object;
        }

        #endregion
    }
}