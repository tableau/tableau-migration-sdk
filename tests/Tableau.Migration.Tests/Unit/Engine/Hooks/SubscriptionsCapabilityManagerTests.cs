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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class SubscriptionsCapabilityManagerTests : AutoFixtureTestBase
    {
        internal readonly SubscriptionsCapabilityManager SubscriptionsCapabilityManager;
        internal readonly Mock<MigrationCapabilities> MockMigrationCapabilities;
        internal readonly Mock<IDestinationEndpoint> MockDestinationEndpoint = new();
        internal readonly Mock<ISharedResourcesLocalizer> MockLocalizer = new();
        internal readonly Mock<ILogger<SubscriptionsCapabilityManager>> MockLogger = new();
        internal readonly Mock<IMigration> MockMigration;

        public SubscriptionsCapabilityManagerTests()
        {
            MockMigrationCapabilities = new Mock<MigrationCapabilities> { CallBase = true };
            MockMigration = new Mock<IMigration>();
                
            SubscriptionsCapabilityManager = new SubscriptionsCapabilityManager(
                MockDestinationEndpoint.Object,
                MockMigrationCapabilities.Object,
                MockLocalizer.Object,
                MockLogger.Object,
                MockMigration.Object);
        }

        public class SetMigrationCapabilityAsync : SubscriptionsCapabilityManagerTests
        {
            [Fact]
            public async Task Subscriptions_enabled_when_disable_subscriptions_is_false()
            {
                // Arrange
                var session = Create<IServerSession>();
                var settings = Create<ISiteSettings>();
                Mock.Get(settings).SetupGet(s => s.DisableSubscriptions).Returns(false);
                Mock.Get(session).SetupGet(s => s.Settings).Returns(settings);
                MockMigration.Setup(m => m.Destination.GetSessionAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IServerSession>.Succeeded(session));

                // Act
                var result = await SubscriptionsCapabilityManager.SetMigrationCapabilityAsync(session, CancellationToken.None);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
            }

            [Fact]
            public async Task Subscriptions_disabled_when_disable_subscriptions_is_true()
            {
                // Arrange
                var session = Create<IServerSession>();
                MockMigration.Setup(m => m.Destination.GetSessionAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IServerSession>.Succeeded(session));

                // Act
                var result = await SubscriptionsCapabilityManager.SetMigrationCapabilityAsync(session, CancellationToken.None);

                // Assert
                Assert.True(result.Success);
                Assert.Single(MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
                Assert.Contains(typeof(ISubscription<>), MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
            }
        }
    }
}
