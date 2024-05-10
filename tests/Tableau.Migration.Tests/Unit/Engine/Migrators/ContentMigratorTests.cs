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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators
{
    public class ContentMigratorTests
    {
        #region - Test Classes -

        public class TestContentMigrator : ContentMigrator<TestContentType>
        {
            public TestContentMigrator(
                IMigrationPipeline pipeline,
                IMigration migration,
                IConfigReader configReader,
                IMigrationHookRunner hooks,
                IContentMappingRunner mappings,
                IContentFilterRunner filters)
                : base(pipeline, migration, configReader, hooks, mappings, filters)
            { }

            new public int BatchSize => base.BatchSize;
        }

        public class ContentMigratorTest : AutoFixtureTestBase
        {
            protected readonly Mock<IConfigReader> MockConfigReader;
            protected readonly Mock<ISourceEndpoint> MockSourceEndpoint;
            protected readonly Mock<IMigrationPipeline> MockPipeline;
            protected readonly Mock<IContentBatchMigrator<TestContentType>> MockBatchMigrator;
            protected readonly Mock<IMigrationManifestContentTypePartitionEditor> MockManifestPartition;
            protected readonly Mock<IMigrationManifestEntryBuilder> MockManifestEntryBuilder;
            protected readonly Mock<IContentFilterRunner> MockFilterRunner;
            protected readonly Mock<IContentMappingRunner> MockMappingRunner;
            protected readonly Mock<IMigrationHookRunner> MockHookRunner;

            protected readonly TestContentMigrator Migrator;

            protected readonly List<TestContentType> SourceContent;
            protected readonly List<ContentMigrationItem<TestContentType>> MigrationItems;

            protected int BatchSize { get; set; }

            protected int NumSourcePages => (int)Math.Ceiling((double)SourceContent.Count / BatchSize);

            public ContentMigratorTest()
            {
                BatchSize = 2;
                MockConfigReader = Freeze<Mock<IConfigReader>>();
                MockConfigReader.Setup(x => x.Get<TestContentType>())
                    .Returns(() => new ContentTypesOptions() { BatchSize = BatchSize });

                SourceContent = new()
                {
                    new(),
                    new(),
                    new(),
                    new()
                };
                MigrationItems = new();

                MockSourceEndpoint = Freeze<Mock<ISourceEndpoint>>();
                MockSourceEndpoint.Setup(x => x.GetPager<TestContentType>(It.IsAny<int>()))
                    .Returns((int pageSize) => new MemoryPager<TestContentType>(SourceContent, pageSize));

                MockManifestEntryBuilder = Freeze<Mock<IMigrationManifestEntryBuilder>>();
                MockManifestEntryBuilder.Setup(x => x.CreateEntries(
                    It.IsAny<IReadOnlyCollection<TestContentType>>(),
                    It.IsAny<Func<TestContentType, IMigrationManifestEntryEditor, ContentMigrationItem<TestContentType>>>()))
                    .Returns((IReadOnlyCollection<TestContentType> c, Func<TestContentType, IMigrationManifestEntryEditor, ContentMigrationItem<TestContentType>> f)
                    =>
                    {
                        return c.Select(i =>
                        {
                            var newItem = f(i, new MigrationManifestEntry(MockManifestEntryBuilder.Object, new ContentReferenceStub(i)));
                            MigrationItems.Add(newItem);
                            return newItem;
                        }).ToImmutableArray();
                    });

                MockManifestPartition = Freeze<Mock<IMigrationManifestContentTypePartitionEditor>>();
                MockManifestPartition.Setup(x => x.GetEntryBuilder(It.IsAny<int>())).Returns(MockManifestEntryBuilder.Object);

                var mockManifestEntries = Freeze<Mock<IMigrationManifestEntryCollectionEditor>>();
                mockManifestEntries.Setup(x => x.GetOrCreatePartition<TestContentType>()).Returns(MockManifestPartition.Object);

                MockBatchMigrator = Freeze<Mock<IContentBatchMigrator<TestContentType>>>();
                MockBatchMigrator.Setup(x => x.MigrateAsync(It.IsAny<ImmutableArray<ContentMigrationItem<TestContentType>>>(), Cancel))
                    .ReturnsAsync((ImmutableArray<ContentMigrationItem<TestContentType>> items, CancellationToken cancel) =>
                    {
                        var itemResults = items
                            .Select(i => ContentItemMigrationResult<TestContentType>.Succeeded(i.ManifestEntry, new()))
                            .Cast<IContentItemMigrationResult<TestContentType>>()
                            .ToImmutableArray();

                        return ContentBatchMigrationResult<TestContentType>.Succeeded(itemResults);
                    });

                MockPipeline = Freeze<Mock<IMigrationPipeline>>();
                MockPipeline.Setup(x => x.GetBatchMigrator<TestContentType>()).Returns(MockBatchMigrator.Object);

                MockFilterRunner = Freeze<Mock<IContentFilterRunner>>();
                MockFilterRunner.Setup(x => x.ExecuteAsync(It.IsAny<IEnumerable<ContentMigrationItem<TestContentType>>>(), Cancel))
                    .ReturnsAsync((IEnumerable<ContentMigrationItem<TestContentType>> items, CancellationToken c) => items);

                MockMappingRunner = Freeze<Mock<IContentMappingRunner>>();
                MockMappingRunner.Setup(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), Cancel))
                    .ReturnsAsync((ContentMappingContext<TestContentType> ctx, CancellationToken c) => ctx);

                MockHookRunner = Freeze<Mock<IMigrationHookRunner>>();
                MockHookRunner.Setup(x => x.ExecuteAsync<IContentBatchMigrationCompletedHook<TestContentType>, IContentBatchMigrationResult<TestContentType>>(It.IsAny<IContentBatchMigrationResult<TestContentType>>(), Cancel))
                    .ReturnsAsync((IContentBatchMigrationResult<TestContentType> result, CancellationToken cancel) => result);

                Migrator = Create<TestContentMigrator>();
            }
        }

        #endregion

        #region - Ctor -

        public class Ctor : ContentMigratorTest
        {
            [Fact]
            public void GetBatchMigratorByContentType()
            {
                MockPipeline.Verify(x => x.GetBatchMigrator<TestContentType>(), Times.Once);
            }
        }

        #endregion

        #region - BatchSize -

        public class BatchSize : ContentMigratorTest
        {
            [Fact]
            public void GetsConfigBatchSize()
            {
                var batchSize = Migrator.BatchSize;

                Assert.Equal(BatchSize, batchSize);
                MockConfigReader.Verify(x => x.Get<TestContentType>(), Times.Once);
            }
        }

        #endregion

        #region - MigrateAsync -

        public class MigrateAsync : ContentMigratorTest
        {
            [Fact]
            public async Task MigratesInBatchesAsync()
            {
                var result = await Migrator.MigrateAsync(Cancel);

                result.AssertSuccess();

                MockSourceEndpoint.Verify(x => x.GetPager<TestContentType>(BatchSize), Times.Once);
                MockManifestPartition.Verify(x => x.GetEntryBuilder(SourceContent.Count), Times.Once);

                Assert.Equal(2, NumSourcePages);

                MockBatchMigrator.Verify(x => x.MigrateAsync(It.IsAny<ImmutableArray<ContentMigrationItem<TestContentType>>>(), Cancel), Times.Exactly(NumSourcePages));
            }

            [Fact]
            public async Task AppliesFiltersAsync()
            {
                MockFilterRunner.Setup(x => x.ExecuteAsync<TestContentType>(It.IsAny<IEnumerable<ContentMigrationItem<TestContentType>>>(), Cancel))
                    .ReturnsAsync((IEnumerable<ContentMigrationItem<TestContentType>> items, CancellationToken c) =>
                    {
                        return items.Skip(1);
                    });

                var result = await Migrator.MigrateAsync(Cancel);

                result.AssertSuccess();

                MockSourceEndpoint.Verify(x => x.GetPager<TestContentType>(BatchSize), Times.Once);
                MockManifestPartition.Verify(x => x.GetEntryBuilder(SourceContent.Count), Times.Once);

                Assert.Equal(2, NumSourcePages);

                MockBatchMigrator.Verify(x => x.MigrateAsync(It.Is<ImmutableArray<ContentMigrationItem<TestContentType>>>(i => i.Length == 1), Cancel), Times.Exactly(NumSourcePages));
            }

            [Fact]
            public async Task FilteredOutItemsMarkedAsSkipped()
            {
                MockFilterRunner.Setup(x => x.ExecuteAsync<TestContentType>(It.IsAny<IEnumerable<ContentMigrationItem<TestContentType>>>(), Cancel))
                    .ReturnsAsync((IEnumerable<ContentMigrationItem<TestContentType>> items, CancellationToken c) =>
                    {
                        return items.Skip(1);
                    });

                var result = await Migrator.MigrateAsync(Cancel);

                result.AssertSuccess();

                MockSourceEndpoint.Verify(x => x.GetPager<TestContentType>(BatchSize), Times.Once);
                MockManifestPartition.Verify(x => x.GetEntryBuilder(SourceContent.Count), Times.Once);

                Assert.Equal(2, NumSourcePages);

                MockBatchMigrator.Verify(x => x.MigrateAsync(It.Is<ImmutableArray<ContentMigrationItem<TestContentType>>>(i => i.Length == 1), Cancel), Times.Exactly(NumSourcePages));

                Assert.Equal(MigrationManifestEntryStatus.Skipped, MigrationItems[0].ManifestEntry.Status);
                Assert.NotEqual(MigrationManifestEntryStatus.Skipped, MigrationItems[1].ManifestEntry.Status);
                Assert.Equal(MigrationManifestEntryStatus.Skipped, MigrationItems[2].ManifestEntry.Status);
                Assert.NotEqual(MigrationManifestEntryStatus.Skipped, MigrationItems[3].ManifestEntry.Status);
            }

            [Fact]
            public async Task ContinueOnBatchFailureAsync()
            {
                MockBatchMigrator.Setup(x => x.MigrateAsync(It.IsAny<ImmutableArray<ContentMigrationItem<TestContentType>>>(), Cancel))
                    .ReturnsAsync((ImmutableArray<ContentMigrationItem<TestContentType>> items, CancellationToken cancel) =>
                    {
                        var itemResults = items
                            .Select(i => ContentItemMigrationResult<TestContentType>.Succeeded(i.ManifestEntry, new()))
                            .Cast<IContentItemMigrationResult<TestContentType>>()
                            .ToImmutableArray();

                        return ContentBatchMigrationResult<TestContentType>.Failed(itemResults, new[] { new Exception() });
                    });

                var result = await Migrator.MigrateAsync(Cancel);

                result.AssertFailure();

                MockSourceEndpoint.Verify(x => x.GetPager<TestContentType>(BatchSize), Times.Once);
                MockManifestPartition.Verify(x => x.GetEntryBuilder(SourceContent.Count), Times.Once);

                Assert.Equal(2, NumSourcePages);
                MockBatchMigrator.Verify(x => x.MigrateAsync(It.IsAny<ImmutableArray<ContentMigrationItem<TestContentType>>>(), Cancel), Times.Exactly(NumSourcePages));
            }

            [Fact]
            public async Task HaltBatchesOnResultFlagAsync()
            {
                MockBatchMigrator.Setup(x => x.MigrateAsync(It.IsAny<ImmutableArray<ContentMigrationItem<TestContentType>>>(), Cancel))
                    .ReturnsAsync((ImmutableArray<ContentMigrationItem<TestContentType>> items, CancellationToken cancel) =>
                    {
                        var itemResults = items
                            .Select(i => ContentItemMigrationResult<TestContentType>.Succeeded(i.ManifestEntry, new()))
                            .Cast<IContentItemMigrationResult<TestContentType>>()
                            .ToImmutableArray();

                        return ContentBatchMigrationResult<TestContentType>.Succeeded(itemResults, performNextBatch: false);
                    });

                var result = await Migrator.MigrateAsync(Cancel);

                result.AssertSuccess();

                MockSourceEndpoint.Verify(x => x.GetPager<TestContentType>(BatchSize), Times.Once);
                MockManifestPartition.Verify(x => x.GetEntryBuilder(SourceContent.Count), Times.Once);

                Assert.Equal(2, NumSourcePages);
                MockBatchMigrator.Verify(x => x.MigrateAsync(It.IsAny<ImmutableArray<ContentMigrationItem<TestContentType>>>(), Cancel), Times.Once());
            }

            [Fact]
            public async Task MapsItemsAsync()
            {
                var result = await Migrator.MigrateAsync(Cancel);

                result.AssertSuccess();

                MockManifestEntryBuilder.Verify(x => x.MapEntriesAsync(It.IsAny<IEnumerable<TestContentType>>(), MockMappingRunner.Object, Cancel), Times.Exactly(NumSourcePages));
            }
        }

        #endregion
    }
}
