//
//  Copyright (c) 2024, Salesforce, Inc.
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators
{
    public class MigratorTests
    {
        public class Execute : AutoFixtureTestBase
        {
            private readonly Mock<IServiceProvider> _mockServices;
            private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
            private readonly Mock<ILogger<Migrator>> _mockLog;
            private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer;
            private readonly Migrator _migrator;

            private readonly IMigrationPlan _plan;
            private readonly IMigrationManifest _previousManifest;

            private readonly Mock<IMigrationInputInitializer> _mockInputInitializer;
            private readonly Mock<ISourceEndpoint> _mockSource;
            private readonly Mock<IDestinationEndpoint> _mockDestination;
            private readonly Mock<IMigrationManifestFactory> _mockManifestFactory;
            private readonly Mock<IMigrationManifestEditor> _mockManifest;
            private readonly Mock<IMigrationManifestEditor> _mockErrorManifest;
            private readonly Mock<IMigrationPipelineFactory> _mockPipelineFactory;
            private readonly Mock<IMigrationPipelineRunner> _mockPipelineRunner;
            private readonly Mock<IMigrationPipeline> _mockPipeline;

            private readonly CancellationToken _cancel;

            public Execute()
            {
                _mockInputInitializer = Freeze<Mock<IMigrationInputInitializer>>();

                _mockSource = Freeze<Mock<ISourceEndpoint>>();
                _mockSource.Setup(x => x.InitializeAsync(_cancel)).ReturnsAsync(Result.Succeeded);

                _mockDestination = Freeze<Mock<IDestinationEndpoint>>();
                _mockDestination.Setup(x => x.InitializeAsync(_cancel)).ReturnsAsync(Result.Succeeded);

                _mockManifestFactory = Freeze<Mock<IMigrationManifestFactory>>();

                _mockManifest = Create<Mock<IMigrationManifestEditor>>();
                _mockErrorManifest = Create<Mock<IMigrationManifestEditor>>();

                _mockManifestFactory.Setup(x => x.Create(It.IsAny<Guid>(), It.IsAny<Guid>()))
                    .Returns(_mockErrorManifest.Object);

                _mockPipelineFactory = Freeze<Mock<IMigrationPipelineFactory>>();
                _mockPipelineRunner = Freeze<Mock<IMigrationPipelineRunner>>();
                _mockPipeline = Freeze<Mock<IMigrationPipeline>>();

                var mockMigration = Create<Mock<IMigration>>();

                _mockServices = Freeze<MockServiceProvider>();
                _mockServices.Setup(x => x.GetService(typeof(IMigrationInputInitializer))).Returns(() => Create<IMigrationInputInitializer>());
                _mockServices.Setup(x => x.GetService(typeof(IMigration))).Returns(mockMigration.Object);

                mockMigration.SetupGet(x => x.Manifest).Returns(_mockManifest.Object);

                _mockServiceScopeFactory = Freeze<Mock<IServiceScopeFactory>>();
                _mockLog = Freeze<Mock<ILogger<Migrator>>>();
                _mockLocalizer = Freeze<Mock<ISharedResourcesLocalizer>>();
                _migrator = Create<Migrator>();

                _plan = Create<IMigrationPlan>();
                _previousManifest = Create<IMigrationManifest>();

                _cancel = new();
            }

            [Fact]
            public async Task AsynchronousWithoutManifestAsync()
            {
                var result = await _migrator.ExecuteAsync(_plan, _cancel);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);
            }

            [Fact]
            public async Task AsynchronousWithPreviousManifestAsync()
            {
                var result = await _migrator.ExecuteAsync(_plan, _previousManifest, _cancel);

                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                _mockServiceScopeFactory.Verify(x => x.CreateScope(), Times.Once);
                _mockServices.Verify(x => x.GetService(typeof(IMigrationInputInitializer)), Times.Once);

                _mockInputInitializer.Verify(x => x.Initialize(_plan, _previousManifest), Times.Once);

                _mockPipelineRunner.Verify(x => x.ExecuteAsync(_mockPipeline.Object, _cancel), Times.Once);
            }

            [Fact]
            public async Task UncaughtExceptionIsFatalErrorAsync()
            {
                var ex = new Exception();
                _mockSource.Setup(x => x.InitializeAsync(_cancel))
                    .Throws(ex);

                var result = await _migrator.ExecuteAsync(_plan, _cancel);

                Assert.Equal(MigrationCompletionStatus.FatalError, result.Status);
                _mockManifest.Verify(x => x.AddErrors(ex), Times.Once);
                Assert.Same(_mockManifest.Object, result.Manifest);
            }

            [Fact]
            public async Task CancellationExceptionIsCancellationAsync()
            {
                var ex = new OperationCanceledException();
                _mockSource.Setup(x => x.InitializeAsync(_cancel))
                    .Throws(ex);

                var result = await _migrator.ExecuteAsync(_plan, _cancel);

                Assert.Equal(MigrationCompletionStatus.Canceled, result.Status);
                _mockManifest.Verify(x => x.AddErrors(ex), Times.Never);
                Assert.Same(_mockManifest.Object, result.Manifest);
            }

            [Fact]
            public async Task CreatesManifestOnErrorBeforeManifestAsync()
            {
                var ex = new Exception();
                _mockInputInitializer.Setup(x => x.Initialize(It.IsAny<IMigrationPlan>(), It.IsAny<IMigrationManifest?>()))
                    .Throws(ex);

                var result = await _migrator.ExecuteAsync(_plan, _cancel);

                Assert.Equal(MigrationCompletionStatus.FatalError, result.Status);

                Assert.Same(_mockErrorManifest.Object, result.Manifest);
                _mockManifest.Verify(x => x.AddErrors(ex), Times.Never);

                _mockManifestFactory.Verify(x => x.Create(_plan.PlanId, It.IsAny<Guid>()), Times.Once);

                _mockErrorManifest.Verify(x => x.AddErrors(ex), Times.Once);
            }

            [Fact]
            public async Task EndpointInitializationErrorAsync()
            {
                var exceptions = new[] { new Exception("foo"), new Exception("bar") };
                _mockSource.Setup(x => x.InitializeAsync(_cancel)).ReturnsAsync(Result.Failed(exceptions));

                var result = await _migrator.ExecuteAsync(_plan, _cancel);

                Assert.Equal(MigrationCompletionStatus.FatalError, result.Status);

                _mockManifest.Verify(x => x.AddErrors(It.IsAny<IEnumerable<IResult>>()), Times.Once);
            }
        }
    }
}
