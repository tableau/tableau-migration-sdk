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
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.InitializeMigration;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.InitializeMigration.Capabilities
{
    public sealed class GroupSetsCapabilityManagerTests
    {
        public abstract class GroupSetsCapabilityManagerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationCapabilitiesEditor> MockCapabilities;

            protected readonly Mock<ISite> MockSourceSite;
            protected readonly Mock<ISite> MockDestinationSite;

            internal readonly GroupSetsCapabilityManager Manager;
            protected readonly IInitializeMigrationContext Context;

            public GroupSetsCapabilityManagerTest()
            {
                MockCapabilities = Freeze<Mock<IMigrationCapabilitiesEditor>>();
                MockCapabilities.Object.ContentTypesDisabledAtDestination.Clear();

                Manager = Create<GroupSetsCapabilityManager>();

                MockSourceSite = Create<Mock<ISite>>();
                MockDestinationSite = Create<Mock<ISite>>();

                var mockCtx = Create<Mock<IInitializeMigrationContext>>();
                mockCtx.SetupGet(x => x.Source.Site).Returns(MockSourceSite.Object);
                mockCtx.SetupGet(x => x.Destination.Site).Returns(MockDestinationSite.Object);

                Context = mockCtx.Object;
            }
        }

        public class SetMigrationCapabilityAsync : GroupSetsCapabilityManagerTest
        {
            [Fact]
            public async Task GroupSetsEnabledAsync()
            {
                MockSourceSite.SetupGet(x => x.GroupSetsEnabled).Returns(true);
                MockDestinationSite.SetupGet(x => x.GroupSetsEnabled).Returns(true);

                var result = await Manager.SetMigrationCapabilityAsync(Context, Cancel);

                Assert.True(result.Success);
                Assert.Empty(MockCapabilities.Object.ContentTypesDisabledAtDestination);
            }

            [Fact]
            public async Task SourceDisabledAsync()
            {
                MockSourceSite.SetupGet(x => x.GroupSetsEnabled).Returns(false);
                MockDestinationSite.SetupGet(x => x.GroupSetsEnabled).Returns(true);

                var result = await Manager.SetMigrationCapabilityAsync(Context, Cancel);

                Assert.True(result.Success);
                Assert.Single(MockCapabilities.Object.ContentTypesDisabledAtDestination);
                Assert.Contains(typeof(IGroupSet), MockCapabilities.Object.ContentTypesDisabledAtDestination);
            }

            [Fact]
            public async Task DestinationDisabledAsync()
            {
                MockSourceSite.SetupGet(x => x.GroupSetsEnabled).Returns(true);
                MockDestinationSite.SetupGet(x => x.GroupSetsEnabled).Returns(false);

                var result = await Manager.SetMigrationCapabilityAsync(Context, Cancel);

                Assert.True(result.Success);
                Assert.Single(MockCapabilities.Object.ContentTypesDisabledAtDestination);
                Assert.Contains(typeof(IGroupSet), MockCapabilities.Object.ContentTypesDisabledAtDestination);
            }
        }
    }
}
