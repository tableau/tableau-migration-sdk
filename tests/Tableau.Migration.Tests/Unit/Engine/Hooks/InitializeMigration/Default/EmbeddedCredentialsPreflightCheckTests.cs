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
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Default;
using Tableau.Migration.Resources;
using Xunit;


namespace Tableau.Migration.Tests.Unit.Engine.Hooks.InitializeMigration.Default
{
    public class EmbeddedCredentialsPreflightCheckTests : AutoFixtureTestBase
    {
        private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer;
        private readonly Mock<ILogger<EmbeddedCredentialsPreflightCheck>> _mockLogger;
        internal Mock<MigrationCapabilities> MockCapabilities;
        internal readonly Mock<IEmbeddedCredentialsCapabilityManager> MockCapabilityManager = new();

        internal readonly EmbeddedCredentialsPreflightCheck Hook;

        public EmbeddedCredentialsPreflightCheckTests()
        {
            _mockLocalizer = new Mock<ISharedResourcesLocalizer>();
            _mockLogger = new Mock<ILogger<EmbeddedCredentialsPreflightCheck>>();
            MockCapabilities = new Mock<MigrationCapabilities> { CallBase = true };

            Hook = new EmbeddedCredentialsPreflightCheck(
                _mockLocalizer.Object,
                _mockLogger.Object,
                MockCapabilities.Object,
                MockCapabilityManager.Object);
        }

        public class ExecuteCheckAsync : EmbeddedCredentialsPreflightCheckTests
        {
            [Fact]
            public async Task Calls_capability_manager()
            {
                var ctx = Create<IInitializeMigrationHookResult>();

                MockCapabilityManager.Setup(cm => cm.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult((IResult)Result.Succeeded()));

                var result = await Hook.ExecuteCheckAsync(ctx, new CancellationToken());

                Assert.NotNull(result);
                Assert.True(result.Success);

                MockCapabilityManager.Verify(
                    x => x.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
            }

            [Fact]
            public async Task Catches_errors()
            {
                var ctx = Create<InitializeMigrationHookResult>();

                MockCapabilityManager.Setup(cm => cm.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult((IResult)Result.Failed(CreateMany<Exception>())));

                var result = await Hook.ExecuteCheckAsync(ctx, new CancellationToken());

                Assert.NotNull(result);
                Assert.False(result.Success);

                MockCapabilityManager.Verify(
                    x => x.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
            }
        }
    }
}
