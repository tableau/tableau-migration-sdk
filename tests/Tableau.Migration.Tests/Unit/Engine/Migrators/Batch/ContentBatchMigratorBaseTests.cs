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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ContentBatchMigratorBaseTests
    {
        #region - Test Classes -

        public class TestContentBatchMigrator : ContentBatchMigratorBase<TestContentType, TestPublishType>
        {
            public Dictionary<ContentMigrationItem<TestContentType>, IResult> PublishResultOverrides { get; }

            public TestContentBatchMigrator(
                IMigrationPipeline pipeline)
                : base(pipeline)
            {
                PublishResultOverrides = new();
            }

            protected override async Task MigrateBatchAsync(ContentMigrationBatch<TestContentType, TestPublishType> batch)
            {
                foreach (var item in batch.Items)
                {
                    await base.MigrateBatchItemAsync(item, batch);
                }
            }

            protected override Task<IResult> MigratePreparedItemAsync(ContentMigrationItem<TestContentType> migrationItem, TestPublishType preparedItem, CancellationToken cancel)
            {
                if (!PublishResultOverrides.TryGetValue(migrationItem, out var result))
                {
                    result = Result.Succeeded();
                }

                if (result.Success)
                {
                    migrationItem.ManifestEntry.SetMigrated();
                }

                return Task.FromResult(result);
            }
        }

        #endregion

        #region - MigrateAsync -

        public class MigrateAsync : ContentBatchMigratorTestBase<TestContentType, TestPublishType>
        {
            private readonly TestContentBatchMigrator _batchMigrator;

            public MigrateAsync()
            {
                _batchMigrator = Create<TestContentBatchMigrator>();
            }

            [Fact]
            public async Task MigratesAllItemsAsync()
            {
                var result = await _batchMigrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                Assert.All(Items, i => Assert.NotNull(result.ItemResults.SingleOrDefault(r => object.ReferenceEquals(i.ManifestEntry, r.ManifestEntry))));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetMigrated(), Times.Once));
            }

            [Fact]
            public async Task RethrowsMigrationCancellationException()
            {
                MockPreparer.Setup(x => x.PrepareAsync(Items[0], It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() =>
                    {
                        CancelSource.Cancel();
                        return (IResult<TestPublishType>)Result<TestPublishType>.Succeeded(new());
                    });

                await Assert.ThrowsAsync<OperationCanceledException>(() => _batchMigrator.MigrateAsync(Items, Cancel));

                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetCanceled(), Times.AtLeastOnce));
            }

            [Fact]
            public async Task CatchesBatchCancellationException()
            {
                MockPreparer.Setup(x => x.PrepareAsync(Items[0], It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException());

                var result = await _batchMigrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();
                Assert.NotEqual(Items.Length, result.ItemResults.Count);

                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetCanceled(), Times.AtLeastOnce));
            }

            [Fact]
            public async Task ItemPreparationFailsAsync()
            {
                var errors = new Exception[] { new(), new() };
                MockPreparer.Setup(x => x.PrepareAsync(Items[0], It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() =>
                    {
                        return (IResult<TestPublishType>)Result<TestPublishType>.Failed(errors);
                    });

                var result = await _batchMigrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                Assert.All(Items, i => Assert.NotNull(result.ItemResults.SingleOrDefault(r => object.ReferenceEquals(i.ManifestEntry, r.ManifestEntry))));

                MockManifestEntries[0].Verify(x => x.SetFailed((IEnumerable<Exception>)errors), Times.Once);
                Assert.All(MockManifestEntries.Skip(1), e => e.Verify(x => x.SetFailed(It.IsAny<IEnumerable<Exception>>()), Times.Never));
            }

            [Fact]
            public async Task ItemMigrationFailsAsync()
            {
                var errors = new Exception[] { new(), new() };
                _batchMigrator.PublishResultOverrides[Items[0]] = Result.Failed(errors);

                var result = await _batchMigrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                Assert.All(Items, i => Assert.NotNull(result.ItemResults.SingleOrDefault(r => object.ReferenceEquals(i.ManifestEntry, r.ManifestEntry))));

                MockManifestEntries[0].Verify(x => x.SetFailed((IEnumerable<Exception>)errors), Times.Once);
                Assert.All(MockManifestEntries.Skip(1), e => e.Verify(x => x.SetFailed(It.IsAny<IEnumerable<Exception>>()), Times.Never));
            }

            [Fact]
            public async Task UncaughtItemExceptionAsync()
            {
                var error = new Exception();
                MockPreparer.Setup(x => x.PrepareAsync(Items[0], It.IsAny<CancellationToken>()))
                    .Throws(error);

                var result = await _batchMigrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                Assert.All(Items, i => Assert.NotNull(result.ItemResults.SingleOrDefault(r => object.ReferenceEquals(i.ManifestEntry, r.ManifestEntry))));

                MockManifestEntries[0].Verify(x => x.SetFailed((IEnumerable<Exception>)new[] { error }), Times.Once);
                Assert.All(MockManifestEntries.Skip(1), e => e.Verify(x => x.SetFailed(It.IsAny<IEnumerable<Exception>>()), Times.Never));
            }
        }

        #endregion
    }
}
