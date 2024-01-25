using System.Collections.Generic;
using System.Linq;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public abstract class BulkDestinationCacheTest<TCache, TContent> : AutoFixtureTestBase
        where TCache : BulkDestinationCache<TContent>
        where TContent : IContentReference
    {
        protected readonly Mock<IMigrationManifestEntryBuilder> MockManifestEntryBuilder;
        protected readonly Mock<IMigrationManifestContentTypePartitionEditor> MockManifestPartition;
        protected readonly Mock<IDestinationEndpoint> MockDestinationEndpoint;

        protected readonly TCache Cache;

        protected int BatchSize { get; set; }

        protected List<TContent> EndpointContent { get; set; }

        protected Dictionary<ContentLocation, Mock<MigrationManifestEntry>> MockManifestEntries = new();

        public BulkDestinationCacheTest()
        {
            EndpointContent = CreateMany<TContent>().ToList();
            BatchSize = EndpointContent.Count / 2;

            MockManifestEntryBuilder = Freeze<Mock<IMigrationManifestEntryBuilder>>();

            SyncManifestEntries();

            MockManifestPartition = Freeze<Mock<IMigrationManifestContentTypePartitionEditor>>();
            MockManifestPartition.Setup(x => x.ByMappedLocation)
                .Returns(() =>
                    MockManifestEntries.Values
                    .Select(m => (IMigrationManifestEntryEditor)m.Object)
                    .ToDictionary(e => e.MappedLocation));

            var mockManifestEditor = Freeze<Mock<IMigrationManifestEditor>>();
            mockManifestEditor.Setup(x => x.Entries.GetOrCreatePartition<TContent>())
                .Returns(MockManifestPartition.Object);

            MockDestinationEndpoint = Freeze<Mock<IDestinationEndpoint>>();
            MockDestinationEndpoint.Setup(x => x.GetPager<TContent>(It.IsAny<int>()))
                .Returns((int batchSize) => new MemoryPager<TContent>(EndpointContent, batchSize));

            var mockConfigReader = Freeze<Mock<IConfigReader>>();
            mockConfigReader.Setup(x => x.Get())
                .Returns(() => new MigrationSdkOptions { BatchSize = BatchSize });

            Cache = Create<TCache>();
        }

        protected void SyncManifestEntries()
        {
            MockManifestEntries = EndpointContent
                // Convert all the TestContentTypes to Mock<MigrationManifestEntry> 
                // MappedLocation is used as a dictionary key, so it shouldn't be mocked
                .Select(x =>
                {
                    var mock = new Mock<MigrationManifestEntry>(MockManifestEntryBuilder.Object, new ContentReferenceStub(x));
                    mock.Setup(m => m.MappedLocation).CallBase();
                    return mock;
                })
                .ToDictionary(m => m.Object.MappedLocation);
        }
    }
}
