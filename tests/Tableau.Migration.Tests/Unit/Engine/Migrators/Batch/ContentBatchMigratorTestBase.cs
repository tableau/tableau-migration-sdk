using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Engine.Preparation;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ContentBatchMigratorTestBase<TContent, TPublish> : AutoFixtureTestBase
        where TContent : class
        where TPublish : class
    {
        protected readonly Mock<IContentItemPreparer<TContent, TPublish>> MockPreparer;
        protected readonly ImmutableArray<Mock<MigrationManifestEntry>> MockManifestEntries;
        protected readonly ImmutableArray<ContentMigrationItem<TContent>> Items;

        public ContentBatchMigratorTestBase()
        {
            MockPreparer = Freeze<Mock<IContentItemPreparer<TContent, TPublish>>>();
            MockPreparer.Setup(x => x.PrepareAsync(It.IsAny<ContentMigrationItem<TContent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Result<TPublish>.Succeeded(Create<TPublish>()));

            var mockPipeline = Freeze<Mock<IMigrationPipeline>>();
            mockPipeline.Setup(x => x.GetItemPreparer<TContent, TPublish>())
                .Returns(MockPreparer.Object);

            MockManifestEntries = CreateMany<Mock<MigrationManifestEntry>>().ToImmutableArray();
            foreach (var manifestEntry in MockManifestEntries)
            {
                manifestEntry.Setup(e => e.Destination).CallBase();
                manifestEntry.Setup(e => e.MappedLocation).CallBase();
                manifestEntry.Setup(e => e.Source).CallBase();
                manifestEntry.Setup(e => e.Status).CallBase();
            }
            Items = MockManifestEntries.Select(me => new ContentMigrationItem<TContent>(Create<TContent>(), me.Object)).ToImmutableArray();
        }
    }
}
