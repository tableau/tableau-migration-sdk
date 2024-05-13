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

using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class MigrationManifestContentTypePartitionTests
    {
        #region - Test Classes -

        public class MigrationManifestContentTypePartitionTest : AutoFixtureTestBase
        {
            protected readonly MockSharedResourcesLocalizer MockLocalizer;
            protected readonly Mock<ILogger<MigrationManifestContentTypePartition>> MockLogger;

            protected readonly MigrationManifestContentTypePartition Partition;

            public MigrationManifestContentTypePartitionTest()
            {
                MockLocalizer = new();
                MockLogger = Freeze<Mock<ILogger<MigrationManifestContentTypePartition>>>();

                Partition = CreateEmpty<TestContentType>();
            }

            protected MigrationManifestContentTypePartition CreateEmpty<T>()
                => new(typeof(T), MockLocalizer.Object, MockLogger.Object);
        }

        #endregion

        #region - Ctor -

        public class Ctor : MigrationManifestContentTypePartitionTest
        {
            [Fact]
            public void Initializes()
            {
                var t = typeof(TestContentType);
                var partition = new MigrationManifestContentTypePartition(t, MockLocalizer.Object, MockLogger.Object);

                Assert.Same(t, partition.ContentType);
            }
        }

        #endregion

        #region - CreateEntries -

        public class CreateEntries : MigrationManifestContentTypePartitionTest
        {
            private void SetupEmptyContentUrls()
            {
                AutoFixture.Register(() =>
                {
                    var mock = Create<Mock<IContentReference>>();
                    mock.SetupGet(x => x.ContentUrl).Returns(string.Empty);
                    return mock.Object;
                });

                AutoFixture.Customize<TestContentType>(c => c.With(x => x.ContentUrl, string.Empty));
            }

            [Fact]
            public void DeepCopiesFromPreviousManifest()
            {
                var entries = CreateMany<IMigrationManifestEntry>().ToImmutableArray();

                Partition.CreateEntries(entries);

                Assert.Equal(entries.Length, Partition.Count);
                Assert.All(entries, e =>
                {
                    Assert.True(Partition.BySourceLocation.ContainsKey(e.Source.Location));
                    Assert.NotSame(e, Partition.BySourceLocation[e.Source.Location]);

                    Assert.True(Partition.BySourceId.ContainsKey(e.Source.Id));
                    Assert.NotSame(e, Partition.BySourceId[e.Source.Id]);

                    Assert.True(Partition.BySourceContentUrl.ContainsKey(e.Source.ContentUrl));
                    Assert.NotSame(e, Partition.BySourceContentUrl[e.Source.ContentUrl]);
                });
            }

            [Fact]
            public void DeepCopiesFromPreviousManifestWithEmptyContentUrl()
            {
                SetupEmptyContentUrls();

                var entries = CreateMany<IMigrationManifestEntry>().ToImmutableArray();

                Partition.CreateEntries(entries);

                Assert.Equal(entries.Length, Partition.Count);
                Assert.Empty(Partition.BySourceContentUrl);
            }

            [Fact]
            public void CreatesFromSourceItems()
            {
                var sourceItems = CreateMany<TestContentType>().ToImmutableArray();

                var results = Partition.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems, (item, entry) => (item, entry));

                Assert.Equal(sourceItems.Length, Partition.Count);
                Assert.Equal(sourceItems.Length, results.Length);
                Assert.All(results, r =>
                {
                    var e = r.Entry;
                    Assert.NotSame(r.SourceItem, e.Source);
                    Assert.Equal(new ContentReferenceStub(r.SourceItem), e.Source);
                    Assert.True(Partition.BySourceLocation.ContainsKey(e.Source.Location));
                    Assert.Same(e, Partition.BySourceLocation[e.Source.Location]);
                    Assert.Same(e, Partition.BySourceId[e.Source.Id]);
                    Assert.Same(e, Partition.BySourceContentUrl[e.Source.ContentUrl]);
                });
            }

            [Fact]
            public void CreatesFromSourceItemsWithEmptyContentUrl()
            {
                SetupEmptyContentUrls();

                var sourceItems = CreateMany<TestContentType>().ToImmutableArray();

                var results = Partition.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems, (item, entry) => (item, entry));

                Assert.Equal(sourceItems.Length, Partition.Count);
                Assert.Equal(sourceItems.Length, results.Length);

                Assert.Empty(Partition.BySourceContentUrl);
            }

            [Fact]
            public void UpdatesSourceReferenceFromPreviousManifest()
            {
                var previous = CreateMany<IMigrationManifestEntry>().ToImmutableArray();

                Partition.CreateEntries(previous);

                var existingEntries = Partition.BySourceLocation.ToImmutableDictionary();

                var sourceItems = existingEntries.Values.Select(e =>
                    {
                        var newSourceItem = Create<TestContentType>();
                        newSourceItem.Location = e.Source.Location;
                        return newSourceItem;
                    }).ToImmutableArray();

                var results = Partition.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems, (item, entry) => (item, entry));

                Assert.All(sourceItems, i =>
                {
                    var existingEntry = existingEntries[i.Location];
                    Assert.True(Partition.BySourceLocation.ContainsKey(i.Location));

                    var newEntry = Partition.BySourceLocation[i.Location];
                    Assert.NotEqual(existingEntry.Source.Id, newEntry.Source.Id);

                    Assert.Same(newEntry, Partition.BySourceId[newEntry.Source.Id]);
                    Assert.DoesNotContain(existingEntry.Source.Id, Partition.BySourceId);

                    Assert.Same(newEntry, Partition.BySourceContentUrl[newEntry.Source.ContentUrl]);
                    Assert.DoesNotContain(existingEntry.Source.ContentUrl, Partition.BySourceContentUrl);

                    var result = results.Single(r => r.SourceItem == i);
                    Assert.Same(newEntry, result.Entry);
                });
            }

            [Fact]
            public void UpdatesSourceReferenceFromPreviousManifestWithEmptyContentUrl()
            {
                SetupEmptyContentUrls();

                var previous = CreateMany<IMigrationManifestEntry>().ToImmutableArray();

                Partition.CreateEntries(previous);

                var existingEntries = Partition.BySourceLocation.ToImmutableDictionary();

                var sourceItems = existingEntries.Values.Select(e =>
                {
                    var newSourceItem = Create<TestContentType>();
                    newSourceItem.Location = e.Source.Location;
                    return newSourceItem;
                }).ToImmutableArray();

                var results = Partition.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems, (item, entry) => (item, entry));

                Assert.Empty(Partition.BySourceContentUrl);
            }
        }

        #endregion

        #region - Count -

        public class Count : MigrationManifestContentTypePartitionTest
        {
            [Fact]
            public void ReturnsCount()
            {
                var entries = CreateMany<IMigrationManifestEntry>().ToImmutableArray();

                Partition.CreateEntries(entries);

                Assert.Equal(entries.Length, Partition.Count);
            }
        }

        #endregion

        #region - GetEnumerator -

        public class GetEnumerator : MigrationManifestContentTypePartitionTest
        {
            [Fact]
            public void GenericTypeReturnsValues()
            {
                Partition.CreateEntries(CreateMany<IMigrationManifestEntry>().ToImmutableArray());

                var values = Partition.BySourceLocation.Values;
                var enumeratorValues = Partition.ToImmutableArray();

                Assert.Equal(values, enumeratorValues);
            }

            [Fact]
            public void NonGenericReturnsValues()
            {
                Partition.CreateEntries(CreateMany<IMigrationManifestEntry>().ToImmutableArray());

                var values = Partition.BySourceLocation.Values;
                var enumeratorValues = ((IEnumerable)Partition).GetEnumerator().ReadAll<IMigrationManifestEntry>().ToImmutableArray();

                Assert.Equal(values, enumeratorValues);
            }
        }

        #endregion

        #region - GetEntryBuilder -

        public class GetEntryBuilder : MigrationManifestContentTypePartitionTest
        {
            [Fact]
            public void AllocatesAndReturns()
            {
                var b = Partition.GetEntryBuilder(1000);

                Assert.Same(Partition, b);
            }
        }

        #endregion

        #region - MapEntriesAsync -

        public class MapEntriesAsync : MigrationManifestContentTypePartitionTest
        {
            private readonly Mock<IContentMappingRunner> _mockMappingRunner;
            private readonly CancellationToken _cancel = new();

            public MapEntriesAsync()
            {
                _mockMappingRunner = Create<Mock<IContentMappingRunner>>();
                _mockMappingRunner.Setup(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), _cancel))
                    .ReturnsAsync((ContentMappingContext<TestContentType> ctx, CancellationToken cancel) => ctx.MapTo(Create<ContentLocation>()));
            }

            [Fact]
            public async Task MapsEntriesAsync()
            {
                const int COUNT = 10;
                var items = CreateMany<TestContentType>(COUNT).ToImmutableArray();

                var entryBuilder = Partition.GetEntryBuilder(COUNT);

                var entries = entryBuilder
                    .CreateEntries(items, (i, e) => new ContentMigrationItem<TestContentType>(i, e));

                var result = await entryBuilder.MapEntriesAsync(items, _mockMappingRunner.Object, _cancel);

                Assert.Same(entryBuilder, result);

                Assert.All(entries, e => Assert.Same(e.ManifestEntry, Partition.ByMappedLocation[e.ManifestEntry.MappedLocation]));
                Assert.All(items, i => _mockMappingRunner.Verify(x => x.ExecuteAsync<TestContentType>(It.Is<ContentMappingContext<TestContentType>>(ctx => ctx.ContentItem == i), _cancel), Times.Once));
            }
        }

        #endregion

        #region - DestinationInfoUpdated -

        public class DestinationInfoUpdated : MigrationManifestContentTypePartitionTest
        {
            [Fact]
            public void NewDestinationInfoUpdatesCache()
            {
                var destinationInfo = Create<IContentReference>();

                var mockEntry = Create<Mock<IMigrationManifestEntryEditor>>();
                mockEntry.SetupGet(x => x.MappedLocation).Returns(destinationInfo.Location);
                mockEntry.SetupGet(x => x.Destination).Returns(destinationInfo);

                var entry = mockEntry.Object;

                var entryBuilder = Partition.GetEntryBuilder(1);
                entryBuilder.DestinationInfoUpdated(entry, null);

                Assert.Same(entry, Partition.ByDestinationId[entry.Destination!.Id]);
                Assert.Same(entry, Partition.ByMappedLocation[entry.MappedLocation]);
            }

            [Fact]
            public void ExistingDestinationInfoRemoved()
            {
                var oldDestinationInfo = Create<IContentReference>();

                var mockEntry = Create<Mock<IMigrationManifestEntryEditor>>();
                mockEntry.SetupGet(x => x.MappedLocation).Returns(oldDestinationInfo.Location);
                mockEntry.SetupGet(x => x.Destination).Returns(oldDestinationInfo);

                var entry = mockEntry.Object;

                var entryBuilder = Partition.GetEntryBuilder(1);
                entryBuilder.DestinationInfoUpdated(entry, null); //Add the old info to the cache.

                var newDestinationInfo = Create<IContentReference>();
                mockEntry.SetupGet(x => x.MappedLocation).Returns(newDestinationInfo.Location);
                mockEntry.SetupGet(x => x.Destination).Returns(newDestinationInfo);

                entryBuilder.DestinationInfoUpdated(entry, oldDestinationInfo);

                Assert.Same(entry, Partition.ByDestinationId[entry.Destination!.Id]);
                Assert.Same(entry, Partition.ByMappedLocation[entry.MappedLocation]);

                Assert.Single(Partition.ByDestinationId);
                Assert.Single(Partition.ByMappedLocation);
            }

            [Fact]
            public void NoDestinationInfoRemoved()
            {
                var oldDestinationInfo = Create<IContentReference>();

                var mockEntry = Create<Mock<IMigrationManifestEntryEditor>>();
                mockEntry.SetupGet(x => x.MappedLocation).Returns(oldDestinationInfo.Location);
                mockEntry.SetupGet(x => x.Destination).Returns(oldDestinationInfo);

                var entry = mockEntry.Object;

                var entryBuilder = Partition.GetEntryBuilder(1);
                entryBuilder.DestinationInfoUpdated(entry, null); //Add the old info to the cache.

                var newDestinationInfo = Create<IContentReference>();
                mockEntry.SetupGet(x => x.MappedLocation).Returns(newDestinationInfo.Location);
                mockEntry.SetupGet(x => x.Destination).Returns((IContentReference?)null);

                entryBuilder.DestinationInfoUpdated(entry, oldDestinationInfo);

                Assert.Empty(Partition.ByDestinationId);
                Assert.Same(entry, Partition.ByMappedLocation[entry.MappedLocation]);
                Assert.Single(Partition.ByMappedLocation);
            }
        }

        #endregion

        #region - Equality -

        public class EqualityTests : MigrationManifestContentTypePartitionTest
        {
            [Fact]
            public void Equal()
            {
                var sourceItems = CreateMany<TestContentType>().ToImmutableArray();
                MigrationManifestContentTypePartition p1 = CreateEmpty<TestContentType>();
                MigrationManifestContentTypePartition p2 = CreateEmpty<TestContentType>();

                p1.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems, (item, entry) => (item, entry));
                p2.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems, (item, entry) => (item, entry));

                Assert.True(p1.Equals(p1));
                Assert.True(p1.Equals(p2));
                Assert.True(p2.Equals(p1));

                Assert.True(p1 == p2);
                Assert.True(p2 == p1);

                Assert.False(p1 != p2);
                Assert.False(p2 != p1);
            }

            [Fact]
            public void DifferentTypeSameSource()
            {
                var sourceItems = CreateMany<TestContentType>().ToImmutableArray();
                MigrationManifestContentTypePartition p1 = CreateEmpty<TestContentType>();
                MigrationManifestContentTypePartition p2 = CreateEmpty<OtherTestContentType>();

                p1.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems, (item, entry) => (item, entry));
                p2.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems, (item, entry) => (item, entry));

                Assert.False(p1.Equals(p2));
                Assert.False(p2.Equals(p1));

                Assert.False(p1 == p2);
                Assert.False(p2 == p1);

                Assert.True(p1 != p2);
                Assert.True(p2 != p1);
            }

            [Fact]
            public void SameTypeDifferentSource()
            {
                var sourceItems1 = CreateMany<TestContentType>().ToImmutableArray();
                var sourceItems2 = CreateMany<TestContentType>().ToImmutableArray();
                MigrationManifestContentTypePartition p1 = CreateEmpty<TestContentType>();
                MigrationManifestContentTypePartition p2 = CreateEmpty<TestContentType>();

                p1.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems1, (item, entry) => (item, entry));
                p2.CreateEntries<TestContentType, (TestContentType SourceItem, IMigrationManifestEntryEditor Entry)>(sourceItems2, (item, entry) => (item, entry));

                Assert.False(p1.Equals(p2));
                Assert.False(p2.Equals(p1));

                Assert.False(p1 == p2);
                Assert.False(p2 == p1);

                Assert.True(p1 != p2);
                Assert.True(p2 != p1);
            }
        }


        #endregion
    }
}
