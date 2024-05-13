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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ParallelContentBatchMigratorBatchBaseTests
    {
        public class TestParallelContentBatchMigrator : ParallelContentBatchMigratorBatchBase<TestContentType, TestPublishType>
        {
            public ContentMigrationBatch<TestContentType, TestPublishType>? CurrentBatch { get; private set; }

            public TestParallelContentBatchMigrator(
                IMigrationPipeline pipeline,
                IConfigReader configReader)
                : base(pipeline, configReader)
            { }

            protected override async Task MigrateBatchAsync(ContentMigrationBatch<TestContentType, TestPublishType> batch)
            {
                CurrentBatch = batch;
                await base.MigrateBatchAsync(batch);
            }

            protected override Task<IResult> MigratePreparedItemAsync(ContentMigrationItem<TestContentType> migrationItem, TestPublishType preparedItem, CancellationToken cancel)
            {
                migrationItem.ManifestEntry.SetMigrated();
                return Task.FromResult<IResult>(Result.Succeeded());
            }
        }

        public class MigrateBatchAsync : ParallelContentBatchMigratorBatchTestBase<TestContentType, TestPublishType>
        {
            private readonly TestParallelContentBatchMigrator _batchMigrator;

            public MigrateBatchAsync()
            {
                _batchMigrator = Create<TestParallelContentBatchMigrator>();
            }

            [Fact]
            public async Task MigratesAllItemsAsync()
            {
                var result = await _batchMigrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                Assert.All(Items, i => Assert.NotNull(result.ItemResults.SingleOrDefault(r => object.ReferenceEquals(i.ManifestEntry, r.ManifestEntry))));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetMigrated(), Times.Once));
            }

            private async Task<IResult<TestPublishType>> PrepareItemWithSuccessAsync(ContentMigrationItem<TestContentType> item, CancellationToken itemCancel)
            {
                itemCancel.ThrowIfCancellationRequested();
                await Task.Delay(30_000, itemCancel);

                itemCancel.ThrowIfCancellationRequested();
                return Result<TestPublishType>.Succeeded(new());
            }

            private async Task<IResult<TestPublishType>> PrepareItemCancelBatchAsync(ContentMigrationItem<TestContentType> item, CancellationToken itemCancel)
            {
                await Task.Delay(500, itemCancel);

                _batchMigrator.CurrentBatch?.BatchCancelSource?.Cancel();

                return Result<TestPublishType>.Succeeded(new());
            }

            // TODO: W-14188246 - Fix Flaky Test.
            // Increasing the timeout configuration helps when the test runs in a machine with limited resources.
            [Fact]
            public async Task BatchCanceledAsync()
            {
                MockPreparer.Setup(x => x.PrepareAsync(It.IsAny<ContentMigrationItem<TestContentType>>(), It.IsAny<CancellationToken>()))
                    .Returns(PrepareItemWithSuccessAsync);

                MockPreparer.Setup(x => x.PrepareAsync(Items[1], It.IsAny<CancellationToken>()))
                    .Returns(PrepareItemCancelBatchAsync);

                CancelSource.CancelAfter(TestCancellationTimeout);

                var stopwatch = Stopwatch.StartNew();
                var result = await _batchMigrator.MigrateAsync(Items, Cancel);
                stopwatch.Stop();

                result.AssertSuccess();
                Assert.True(stopwatch.ElapsedMilliseconds < 25_000);

                Assert.All(MockManifestEntries, mockEntry => mockEntry.Verify(x => x.SetFailed(It.IsAny<IEnumerable<Exception>>()), Times.Never));

                Assert.Equal(result.ItemResults.Count, Items.Length);

                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetCanceled(), Times.AtLeastOnce));
            }
        }
    }
}
