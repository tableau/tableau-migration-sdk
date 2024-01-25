using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish.Default
{
    public class OwnerItemPostPublishHookTests
    {
        public class OwnerUpdateContentType : TestContentType, IRequiresOwnerUpdate
        {
            public IContentReference Owner { get; set; } = null!;
        }

        public class OwnerContentType : TestContentType, IWithOwner
        {
            public IContentReference Owner { get; set; } = null!;
        }

        public class OwnerItemPostPublishHookTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration = new();
            protected readonly Mock<IDestinationEndpoint> MockDestinationEndpoint = new();

            protected readonly OwnerItemPostPublishHook<OwnerUpdateContentType, OwnerContentType> Hook;

            public OwnerItemPostPublishHookTest()
            {
                MockMigration.SetupGet(m => m.Destination).Returns(MockDestinationEndpoint.Object);

                Hook = new(MockMigration.Object);
            }
        }

        public class ExecuteAsync : OwnerItemPostPublishHookTest
        {
            [Fact]
            public async Task Succeeds()
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();
                var sourceItem = Create<OwnerUpdateContentType>();
                var destinationItem = Create<OwnerContentType>();

                var context = new ContentItemPostPublishContext<OwnerUpdateContentType, OwnerContentType>(manifestEntry, sourceItem, destinationItem);

                MockDestinationEndpoint.Setup(e => e.UpdateOwnerAsync<OwnerUpdateContentType>(destinationItem, sourceItem.Owner, Cancel))
                    .ReturnsAsync(Result.Succeeded());

                var result = await Hook.ExecuteAsync(context, Cancel);

                Assert.Same(context, result);
            }
        }
    }
}
