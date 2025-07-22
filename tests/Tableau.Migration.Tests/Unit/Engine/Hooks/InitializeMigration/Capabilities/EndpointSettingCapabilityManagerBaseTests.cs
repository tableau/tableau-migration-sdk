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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine.Hooks.InitializeMigration;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.InitializeMigration.Capabilities
{
    public sealed class EndpointSettingCapabilityManagerBaseTests
    {
        public class TestManager : EndpointSettingCapabilityManagerBase
        {
            public TestManager(IMigrationCapabilitiesEditor capabilities,
                ISharedResourcesLocalizer localizer, ILogger logger)
                : base(capabilities, localizer, logger)
            { }

            public HashSet<IEndpointPreflightContext> DisabledEndpoints { get; } = new();

            public IEnumerable<Type> PublicCapabilityContentTypes { get; set; } = [];

            protected override Type DisplayCapabilityContentTypes => typeof(TestContentType);

            protected override IEnumerable<Type> CapabilityContentTypes => PublicCapabilityContentTypes;

            protected override bool GetEndpointDisabledSetting(IEndpointPreflightContext ctx)
                => DisabledEndpoints.Contains(ctx);
        }

        public abstract class EndpointSettingCapabilityManagerBaseTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationCapabilitiesEditor> MockCapabilities;
            protected readonly IInitializeMigrationContext Context;
            protected readonly TestManager Manager;

            public EndpointSettingCapabilityManagerBaseTest()
            {
                MockCapabilities = Freeze<Mock<IMigrationCapabilitiesEditor>>();
                MockCapabilities.Object.ContentTypesDisabledAtDestination.Clear();

                Context = Create<IInitializeMigrationContext>();
                Manager = Create<TestManager>();
                Manager.PublicCapabilityContentTypes = [typeof(TestContentType), typeof(TestPublishType)];
            }
        }

        public class SetMigrationCapabilityAsync : EndpointSettingCapabilityManagerBaseTest
        {
            [Fact]
            public async Task EndpointsEnabledAsync()
            {
                var result = await Manager.SetMigrationCapabilityAsync(Context, Cancel);

                Assert.True(result.Success);
                Assert.Empty(MockCapabilities.Object.ContentTypesDisabledAtDestination);
            }

            [Fact]
            public async Task SourceDisabledAsync()
            {
                Manager.DisabledEndpoints.Add(Context.Source);

                var result = await Manager.SetMigrationCapabilityAsync(Context, Cancel);

                Assert.True(result.Success);
                Assert.Equal(Manager.PublicCapabilityContentTypes.ToArray(), MockCapabilities.Object.ContentTypesDisabledAtDestination.ToArray());
            }

            [Fact]
            public async Task DestinationDisabledAsync()
            {
                Manager.DisabledEndpoints.Add(Context.Destination);

                var result = await Manager.SetMigrationCapabilityAsync(Context, Cancel);

                Assert.True(result.Success);
                Assert.Equal(Manager.PublicCapabilityContentTypes.ToArray(), MockCapabilities.Object.ContentTypesDisabledAtDestination.ToArray());
            }

            [Fact]
            public async Task BothEndpointsDisabledAsync()
            {
                Manager.DisabledEndpoints.Add(Context.Source);
                Manager.DisabledEndpoints.Add(Context.Destination);

                var result = await Manager.SetMigrationCapabilityAsync(Context, Cancel);

                Assert.True(result.Success);
                Assert.Equal(Manager.PublicCapabilityContentTypes.ToArray(), MockCapabilities.Object.ContentTypesDisabledAtDestination.ToArray());
            }
        }
    }
}
