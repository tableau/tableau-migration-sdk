using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class MappedUserTransformerTests
    {
        public abstract class MappedUserTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationPipeline> MockMigrationPipeline = new();
            protected readonly Mock<ILogger<MappedUserTransformer>> MockLogger = new();
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();
            protected readonly Mock<IMappedContentReferenceFinder<IUser>> MockUserContentFinder = new();

            protected readonly MappedUserTransformer Transformer;

            public MappedUserTransformerTest()
            {
                MockMigrationPipeline.Setup(p => p.CreateDestinationFinder<IUser>()).Returns(MockUserContentFinder.Object);

                Transformer = new(MockMigrationPipeline.Object, MockLogger.Object, MockSharedResourcesLocalizer.Object);
            }
        }

        public class ExecuteAsync : MappedUserTransformerTest
        {
            [Fact]
            public async Task Returns_null_when_user_not_found()
            {
                var result = await Transformer.ExecuteAsync(Create<IContentReference>(), Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task Returns_destination_user_when_found()
            {
                var sourceUser = Create<IContentReference>();
                var destinationUser = Create<IContentReference>();

                MockUserContentFinder.Setup(f => f.FindDestinationReferenceAsync(sourceUser.Location, Cancel)).ReturnsAsync(destinationUser);

                var result = await Transformer.ExecuteAsync(sourceUser, Cancel);

                Assert.Same(destinationUser, result);
            }
        }
    }
}
