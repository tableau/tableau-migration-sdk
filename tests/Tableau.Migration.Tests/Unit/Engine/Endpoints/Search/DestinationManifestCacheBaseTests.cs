using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class DestinationManifestCacheBaseTests
    {
        #region - Test Classes -

        public class TestDestinationManifestCache : DestinationManifestCacheBase<TestContentType>
        {
            public Func<ContentLocation, IEnumerable<ContentReferenceStub>> SearchStoreByLocationAction { get; set; }

            public Func<Guid, IEnumerable<ContentReferenceStub>> SearchStoreByIdAction { get; set; }

            public int SearchStoreCalls { get; private set; }

            public TestDestinationManifestCache(IMigrationManifestEditor manifest)
                : base(manifest)
            {
                SearchStoreByLocationAction = loc => Enumerable.Empty<ContentReferenceStub>();
                SearchStoreByIdAction = id => Enumerable.Empty<ContentReferenceStub>();
            }

            protected override ValueTask<IEnumerable<ContentReferenceStub>> SearchStoreAsync(ContentLocation searchLocation, CancellationToken cancel)
            {
                SearchStoreCalls++;
                return ValueTask.FromResult(SearchStoreByLocationAction(searchLocation));
            }

            protected override ValueTask<IEnumerable<ContentReferenceStub>> SearchStoreAsync(Guid searchId, CancellationToken cancel)
            {
                SearchStoreCalls++;
                return ValueTask.FromResult(SearchStoreByIdAction(searchId));
            }
        }

        public class DestinationManifestCacheBaseTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationManifestEntryCollectionEditor> MockEntriesCollection;
            protected readonly TestDestinationManifestCache Cache;
            protected readonly List<MigrationManifestEntry> ManifestEntries;

            public DestinationManifestCacheBaseTest()
            {
                ManifestEntries = new();

                var mockEntries = Create<Mock<IMigrationManifestContentTypePartitionEditor>>();
                mockEntries.Setup(x => x.ByMappedLocation)
                    .Returns(() => ManifestEntries.ToImmutableDictionary(e => e.MappedLocation, e => (IMigrationManifestEntryEditor)e));
                mockEntries.Setup(x => x.ByDestinationId)
                    .Returns(
                        () => ManifestEntries
                        .Where(e => e.Destination is not null)
                        .ToImmutableDictionary(e => e.Destination!.Id, e => (IMigrationManifestEntryEditor)e)
                    );

                MockEntriesCollection = Freeze<Mock<IMigrationManifestEntryCollectionEditor>>();
                MockEntriesCollection.Setup(x => x.GetOrCreatePartition<TestContentType>())
                    .Returns(mockEntries.Object);

                Cache = Create<TestDestinationManifestCache>();
            }
        }

        #endregion

        #region - SearchAsync (Location) -

        public class SearchAsyncByLocation : DestinationManifestCacheBaseTest
        {
            [Fact]
            public async Task SearchesManifestDestinationInfoAsync()
            {
                var entry = Create<MigrationManifestEntry>();
                entry.DestinationFound(Create<IContentReference>());
                ManifestEntries.Add(entry);

                var resultRef = await Cache.ForLocationAsync(entry.MappedLocation, Cancel);

                Assert.NotNull(resultRef);
                Assert.Equal(new ContentReferenceStub(entry.Destination!), resultRef);

                Assert.Equal(0, Cache.SearchStoreCalls);

                MockEntriesCollection.Verify(x => x.GetOrCreatePartition<TestContentType>(), Times.Once);
            }

            [Fact]
            public async Task SearchesStoreAsync()
            {
                var searchRef = Create<ContentReferenceStub>();
                var searchLoc = searchRef.Location;

                var foundRefs = CreateMany<ContentReferenceStub>()
                    .Append(searchRef)
                    .ToImmutableArray();

                Cache.SearchStoreByLocationAction = (loc) => foundRefs;

                var resultRef = await Cache.ForLocationAsync(searchLoc, Cancel);

                Assert.NotNull(resultRef);
                Assert.Equal(searchRef, resultRef);

                Assert.Equal(1, Cache.SearchStoreCalls);

                MockEntriesCollection.Verify(x => x.GetOrCreatePartition<TestContentType>(), Times.Once);
            }

            [Fact]
            public async Task NotFoundInStoreAsync()
            {
                var searchRef = Create<IContentReference>();
                var searchLoc = searchRef.Location;

                var resultRef = await Cache.ForLocationAsync(searchLoc, Cancel);

                Assert.Null(resultRef);

                Assert.Equal(1, Cache.SearchStoreCalls);

                MockEntriesCollection.Verify(x => x.GetOrCreatePartition<TestContentType>(), Times.Once);
            }
        }

        #endregion

        #region - SearchAsync (Id) -

        public class SearchAsyncById : DestinationManifestCacheBaseTest
        {
            [Fact]
            public async Task SearchesManifestDestinationInfoAsync()
            {
                var entry = Create<MigrationManifestEntry>();
                entry.DestinationFound(Create<IContentReference>());
                ManifestEntries.Add(entry);

                var resultRef = await Cache.ForIdAsync(entry.Destination!.Id, Cancel);

                Assert.NotNull(resultRef);
                Assert.Equal(new ContentReferenceStub(entry.Destination!), resultRef);

                Assert.Equal(0, Cache.SearchStoreCalls);

                MockEntriesCollection.Verify(x => x.GetOrCreatePartition<TestContentType>(), Times.Once);
            }

            [Fact]
            public async Task SearchesStoreAsync()
            {
                var searchRef = Create<ContentReferenceStub>();

                var foundRefs = CreateMany<ContentReferenceStub>()
                    .Append(searchRef)
                    .ToImmutableArray();

                Cache.SearchStoreByIdAction = (id) => foundRefs;

                var resultRef = await Cache.ForIdAsync(searchRef.Id, Cancel);

                Assert.NotNull(resultRef);
                Assert.Equal(searchRef, resultRef);

                Assert.Equal(1, Cache.SearchStoreCalls);

                MockEntriesCollection.Verify(x => x.GetOrCreatePartition<TestContentType>(), Times.Once);
            }

            [Fact]
            public async Task NotFoundInStoreAsync()
            {
                var searchRef = Create<IContentReference>();

                var resultRef = await Cache.ForIdAsync(searchRef.Id, Cancel);

                Assert.Null(resultRef);

                Assert.Equal(1, Cache.SearchStoreCalls);

                MockEntriesCollection.Verify(x => x.GetOrCreatePartition<TestContentType>(), Times.Once);
            }
        }

        #endregion
    }
}
