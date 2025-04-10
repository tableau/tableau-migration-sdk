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
using Tableau.Migration.Engine.Hooks.InitializeMigration;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.InitializeMigration
{
    public class InitializeMigrationCapabilityHookBaseTests
    {
        private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer;
        private readonly Mock<ILogger<IInitializeMigrationHook>> _mockLogger;
        private readonly Mock<MigrationCapabilities> _mockCapabilities;

        public InitializeMigrationCapabilityHookBaseTests()
        {
            _mockLocalizer = new Mock<ISharedResourcesLocalizer>();
            _mockLogger = new Mock<ILogger<IInitializeMigrationHook>>();
            _mockCapabilities = new Mock<MigrationCapabilities>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldLogChanges_WhenCapabilitiesChange()
        {
            // Arrange
            var hook = new TestInitializeWithChangesMigrationHook(_mockLocalizer.Object, _mockLogger.Object, _mockCapabilities.Object);
            var ctx = Mock.Of<IInitializeMigrationHookResult>();
            var cancelToken = new CancellationToken();

            // Act
            await hook.ExecuteAsync(ctx, cancelToken);

            // Assert
            _mockLogger.Verify(
                static x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldLogChanges_WhenCapabilitiesDidNotChange()
        {
            // Arrange
            var hook = new TestInitializeWithoutChangesMigrationHook(_mockLocalizer.Object, _mockLogger.Object, _mockCapabilities.Object);
            var ctx = Mock.Of<IInitializeMigrationHookResult>();
            var cancelToken = new CancellationToken();

            // Act
            await hook.ExecuteAsync(ctx, cancelToken);

            // Assert
            _mockLogger.Verify(
                static x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        private class TestInitializeWithChangesMigrationHook : InitializeMigrationCapabilityHookBase
        {
            public TestInitializeWithChangesMigrationHook(
                ISharedResourcesLocalizer? localizer,
                ILogger<IInitializeMigrationHook>? logger,
                IMigrationCapabilitiesEditor capabilities)
                : base(localizer, logger, capabilities)
            { }

            public override Task<IInitializeMigrationHookResult?> ExecuteCheckAsync(IInitializeMigrationHookResult ctx, CancellationToken cancel)
            {
                Capabilities.PreflightCheckExecuted = !Capabilities.PreflightCheckExecuted;
                return Task.FromResult<IInitializeMigrationHookResult?>(ctx);
            }
        }

        private class TestInitializeWithoutChangesMigrationHook : InitializeMigrationCapabilityHookBase
        {
            public TestInitializeWithoutChangesMigrationHook(
                ISharedResourcesLocalizer? localizer,
                ILogger<IInitializeMigrationHook>? logger,
                IMigrationCapabilitiesEditor capabilities)
                : base(localizer, logger, capabilities)
            { }

            public override Task<IInitializeMigrationHookResult?> ExecuteCheckAsync(IInitializeMigrationHookResult ctx, CancellationToken cancel)
            {
                return Task.FromResult<IInitializeMigrationHookResult?>(ctx);
            }
        }
    }
}
