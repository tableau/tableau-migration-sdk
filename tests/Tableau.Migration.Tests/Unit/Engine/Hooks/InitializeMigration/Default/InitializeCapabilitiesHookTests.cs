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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.InitializeMigration;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.InitializeMigration.Default
{
    public sealed class InitializeCapabilitiesHookTests
    {
        public abstract class InitializeCapabilitiesHookTest : AutoFixtureTestBase
        {
            internal readonly MigrationCapabilities Capabilities = new();
            protected readonly MockSharedResourcesLocalizer Localizer = new();
            protected readonly Mock<ILogger<InitializeCapabilitiesHook>> MockLogger;

            protected List<IMigrationCapabilityManager> CapabilityManagers { get; set; } = new();

            public InitializeCapabilitiesHookTest()
            {
                MockLogger = Create<Mock<ILogger<InitializeCapabilitiesHook>>>();
            }

            protected InitializeCapabilitiesHook CreateHook()
            {
                return new(Capabilities, CapabilityManagers, Localizer.Object, MockLogger.Object);
            }
        }

        public sealed class ExecuteAsync : InitializeCapabilitiesHookTest
        {
            [Fact]
            public async Task RunsAllCapabilityManagersAsync()
            {
                var mockCtx = Create<Mock<IInitializeMigrationHookResult>>();
                var ctx = mockCtx.Object;

                var mockManager1 = Create<Mock<IMigrationCapabilityManager>>();
                mockManager1.Setup(x => x.SetMigrationCapabilityAsync(ctx, Cancel))
                    .ReturnsAsync(() =>
                    {
                        Capabilities.EmbeddedCredentialsDisabled = true;
                        return Result.Succeeded();
                    });

                var mockManager2 = Create<Mock<IMigrationCapabilityManager>>();
                mockManager2.Setup(x => x.SetMigrationCapabilityAsync(ctx, Cancel))
                    .ReturnsAsync(() =>
                    {
                        Capabilities.ContentTypesDisabledAtDestination.Add(typeof(IWorkbook));
                        return Result.Succeeded();
                    });

                CapabilityManagers = [mockManager1.Object, mockManager2.Object];

                var result = await CreateHook().ExecuteAsync(ctx, Cancel);

                mockCtx.Verify(x => x.ToFailure(It.IsAny<IEnumerable<Exception>>()), Times.Never);
                MockLogger.VerifyLogging(LogLevel.Debug, Times.Exactly(2));

                mockManager1.Verify(x => x.SetMigrationCapabilityAsync(ctx, Cancel), Times.Once);
                mockManager2.Verify(x => x.SetMigrationCapabilityAsync(ctx, Cancel), Times.Once);
            }

            [Fact]
            public async Task CapabilityManagerFailsAsync()
            {
                var mockCtx = Create<Mock<IInitializeMigrationHookResult>>();
                var ctx = mockCtx.Object;

                var mockManager1 = Create<Mock<IMigrationCapabilityManager>>();
                mockManager1.Setup(x => x.SetMigrationCapabilityAsync(ctx, Cancel))
                    .ReturnsAsync(() =>
                    {
                        Capabilities.EmbeddedCredentialsDisabled = true;
                        return Result.Succeeded();
                    });

                var ex = new Exception();
                var mockManager2 = Create<Mock<IMigrationCapabilityManager>>();
                mockManager2.Setup(x => x.SetMigrationCapabilityAsync(ctx, Cancel))
                    .ReturnsAsync(() =>
                    {
                        Capabilities.ContentTypesDisabledAtDestination.Add(typeof(IWorkbook));
                        return Result.Failed(ex);
                    });

                CapabilityManagers = [mockManager1.Object, mockManager2.Object];

                var result = await CreateHook().ExecuteAsync(ctx, Cancel);

                mockCtx.Verify(x => x.ToFailure(It.IsAny<IEnumerable<Exception>>()), Times.Once);
                MockLogger.VerifyLogging(LogLevel.Debug, Times.Exactly(2));
            }

            [Fact]
            public async Task NoChangesAsync()
            {
                CapabilityManagers.Clear();

                var mockCtx = Create<Mock<IInitializeMigrationHookResult>>();

                var result = await CreateHook().ExecuteAsync(mockCtx.Object, Cancel);

                mockCtx.Verify(x => x.ToFailure(It.IsAny<IEnumerable<Exception>>()), Times.Never);
                MockLogger.VerifyLogging(LogLevel.Debug, Times.Once);
            }
        }
    }
}
