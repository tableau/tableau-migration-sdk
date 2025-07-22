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
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.InitializeMigration;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.InitializeMigration.Capabilities
{
    public class SubscriptionsCapabilityManagerTests : AutoFixtureTestBase
    {
        internal readonly SubscriptionsCapabilityManager SubscriptionsCapabilityManager;
        internal readonly Mock<MigrationCapabilities> MockMigrationCapabilities;
        internal readonly Mock<ISharedResourcesLocalizer> MockLocalizer = new();
        internal readonly Mock<ILogger<SubscriptionsCapabilityManager>> MockLogger = new();

        protected readonly Mock<ISiteSettings> MockSourceSettings;
        protected readonly Mock<ISiteSettings> MockDestinationSettings;

        protected readonly IInitializeMigrationContext Context;

        public SubscriptionsCapabilityManagerTests()
        {
            MockMigrationCapabilities = new Mock<MigrationCapabilities> { CallBase = true };
                
            SubscriptionsCapabilityManager = new SubscriptionsCapabilityManager(
                MockMigrationCapabilities.Object,
                MockLocalizer.Object,
                MockLogger.Object);

            MockSourceSettings = Create<Mock<ISiteSettings>>();
            MockDestinationSettings = Create<Mock<ISiteSettings>>();

            var mockCtx = Create<Mock<IInitializeMigrationContext>>();
            mockCtx.SetupGet(x => x.Source.Session.Settings).Returns(MockSourceSettings.Object);
            mockCtx.SetupGet(x => x.Destination.Session.Settings).Returns(MockDestinationSettings.Object);

            Context = mockCtx.Object;
        }

        public class SetMigrationCapabilityAsync : SubscriptionsCapabilityManagerTests
        {
            [Fact]
            public async Task SubscriptionsEnabledAsync()
            {
                // Arrange
                MockSourceSettings.SetupGet(x => x.DisableSubscriptions).Returns(false);
                MockDestinationSettings.SetupGet(x => x.DisableSubscriptions).Returns(false);

                // Act
                var result = await SubscriptionsCapabilityManager.SetMigrationCapabilityAsync(Context, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
            }

            [Fact]
            public async Task SourceDisabledAsync()
            {
                // Arrange
                MockSourceSettings.SetupGet(x => x.DisableSubscriptions).Returns(true);
                MockDestinationSettings.SetupGet(x => x.DisableSubscriptions).Returns(false);

                // Act
                var result = await SubscriptionsCapabilityManager.SetMigrationCapabilityAsync(Context, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(2, MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination.Count);
                Assert.Contains(typeof(IServerSubscription), MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
                Assert.Contains(typeof(ICloudSubscription), MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
            }

            [Fact]
            public async Task DestinationDisabledAsync()
            {
                // Arrange
                MockSourceSettings.SetupGet(x => x.DisableSubscriptions).Returns(false);
                MockDestinationSettings.SetupGet(x => x.DisableSubscriptions).Returns(true);

                // Act
                var result = await SubscriptionsCapabilityManager.SetMigrationCapabilityAsync(Context, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(2, MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination.Count);
                Assert.Contains(typeof(IServerSubscription), MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
                Assert.Contains(typeof(ICloudSubscription), MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
            }

            [Fact]
            public async Task AllEndpointsDisabledAsync()
            {
                // Arrange
                MockSourceSettings.SetupGet(x => x.DisableSubscriptions).Returns(true);
                MockDestinationSettings.SetupGet(x => x.DisableSubscriptions).Returns(true);

                // Act
                var result = await SubscriptionsCapabilityManager.SetMigrationCapabilityAsync(Context, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(2, MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination.Count);
                Assert.Contains(typeof(IServerSubscription), MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
                Assert.Contains(typeof(ICloudSubscription), MockMigrationCapabilities.Object.ContentTypesDisabledAtDestination);
            }
        }
    }
}
