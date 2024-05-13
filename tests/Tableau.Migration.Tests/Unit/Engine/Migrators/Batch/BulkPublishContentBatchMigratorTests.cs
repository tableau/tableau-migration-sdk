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
using Moq;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Migrators.Batch;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class BulkPublishContentBatchMigratorTests
    {
        public class MigrateBatchAsync : ParallelContentBatchMigratorBatchTestBase<TestContentType, TestPublishType>
        {
            private readonly Mock<IDestinationEndpoint> _mockDestination;
            private readonly Mock<IMigrationHookRunner> _mockHookRunner;
            private readonly BulkPublishContentBatchMigrator<TestContentType, TestPublishType> _migrator;

            public MigrateBatchAsync()
            {
                _mockDestination = Freeze<Mock<IDestinationEndpoint>>();
                _mockDestination.Setup(x => x.PublishBatchAsync(It.IsAny<IEnumerable<TestPublishType>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => Result.Succeeded());

                _mockHookRunner = Freeze<Mock<IMigrationHookRunner>>();
                _mockHookRunner.Setup(r => r.ExecuteAsync<IBulkPostPublishHook<TestPublishType>, BulkPostPublishContext<TestPublishType>>(
                    It.IsAny<BulkPostPublishContext<TestPublishType>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((BulkPostPublishContext<TestPublishType> ctx, CancellationToken c) => ctx);

                _migrator = Create<BulkPublishContentBatchMigrator<TestContentType, TestPublishType>>();
            }

            [Fact]
            public async Task BulkPublishesAfterPreparationAsync()
            {
                var result = await _migrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                _mockDestination.Verify(x => x.PublishBatchAsync(It.IsAny<IEnumerable<TestPublishType>>(), It.IsAny<CancellationToken>()), Times.Once);

                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetMigrated(), Times.Once));

                _mockHookRunner.Verify(r =>
                    r.ExecuteAsync<IBulkPostPublishHook<TestPublishType>, BulkPostPublishContext<TestPublishType>>(
                        It.IsAny<BulkPostPublishContext<TestPublishType>>(),
                        It.IsAny<CancellationToken>()),
                        Times.Once);
            }

            [Fact]
            public async Task BatchCanceledInPreparationAsync()
            {
                MockPreparer.Setup(x => x.PrepareAsync(Items[0], It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException());

                var result = await _migrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                _mockDestination.Verify(x => x.PublishBatchAsync(It.IsAny<IEnumerable<TestPublishType>>(), It.IsAny<CancellationToken>()), Times.Never);
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetCanceled(), Times.AtLeastOnce));
            }

            [Fact]
            public async Task BulkPublishFailsAsync()
            {
                var errors = new Exception[] { new(), new() };
                _mockDestination.Setup(x => x.PublishBatchAsync(It.IsAny<IEnumerable<TestPublishType>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => Result.Failed(errors));

                var result = await _migrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                _mockDestination.Verify(x => x.PublishBatchAsync(It.IsAny<IEnumerable<TestPublishType>>(), It.IsAny<CancellationToken>()), Times.Once);

                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetMigrated(), Times.Never));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetFailed((IEnumerable<Exception>)errors), Times.Once));
            }
        }
    }
}
