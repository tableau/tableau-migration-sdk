using System.Threading;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public class ContentItemPreparerTestBase<TPublish> : AutoFixtureTestBase
    {
        protected readonly Mock<IContentTransformerRunner> MockTransformerRunner;
        protected readonly Mock<IMigrationManifestEntryEditor> MockManifestEntry;
        protected readonly Mock<IMappedContentReferenceFinder<IProject>> MockProjectFinder;
        protected readonly Mock<IContentFileStore> MockFileStore;
        protected readonly ContentMigrationItem<TestContentType> Item;

        protected ContentLocation MappedLocation { get; set; }

        public ContentItemPreparerTestBase()
        {
            MockTransformerRunner = Freeze<Mock<IContentTransformerRunner>>();
            MockTransformerRunner.Setup(x => x.ExecuteAsync(It.IsAny<TPublish>(), Cancel))
                .ReturnsAsync((TPublish item, CancellationToken cancel) => item);

            MappedLocation = Create<ContentLocation>();

            MockManifestEntry = Freeze<Mock<IMigrationManifestEntryEditor>>();
            MockManifestEntry.SetupGet(x => x.MappedLocation).Returns(() => MappedLocation);

            MockProjectFinder = Freeze<Mock<IMappedContentReferenceFinder<IProject>>>();
            MockProjectFinder.Setup(x => x.FindDestinationReferenceAsync(It.IsAny<ContentLocation>(), Cancel))
                    .ReturnsAsync((IContentReference?)null);

            var mockPipeline = Freeze<Mock<IMigrationPipeline>>();
            mockPipeline.Setup(x => x.CreateDestinationFinder<IProject>())
                .Returns(MockProjectFinder.Object);

            MockFileStore = Freeze<Mock<IContentFileStore>>();

            Item = Create<ContentMigrationItem<TestContentType>>();
        }
    }
}
