using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class IMigrationExtensionsTests
    {
        public abstract class IMigrationExtensionsTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration = new();
        }

        public class TryGetSourceApiEndpoint : IMigrationExtensionsTest
        {
            [Fact]
            public void Returns_false_when_not_Api_endpoint()
            {
                MockMigration.SetupGet(m => m.Source).Returns(new Mock<ISourceEndpoint>().Object);

                Assert.False(MockMigration.Object.TryGetSourceApiEndpoint(out var endpoint));
                Assert.Null(endpoint);
            }

            [Fact]
            public void Returns_true_when_Api_endpoint()
            {
                var sourceEndpoint = new Mock<ISourceApiEndpoint>().Object;

                MockMigration.SetupGet(m => m.Source).Returns(sourceEndpoint);

                Assert.True(MockMigration.Object.TryGetSourceApiEndpoint(out var endpoint));
                Assert.Same(sourceEndpoint, endpoint);
            }
        }

        public class TryGetDestinationApiEndpoint : IMigrationExtensionsTest
        {
            [Fact]
            public void Returns_false_when_not_Api_endpoint()
            {
                MockMigration.SetupGet(m => m.Destination).Returns(new Mock<IDestinationEndpoint>().Object);

                Assert.False(MockMigration.Object.TryGetDestinationApiEndpoint(out var endpoint));
                Assert.Null(endpoint);
            }

            [Fact]
            public void Returns_true_when_Api_endpoint()
            {
                var destinationEndpoint = new Mock<IDestinationApiEndpoint>().Object;

                MockMigration.SetupGet(m => m.Destination).Returns(destinationEndpoint);

                Assert.True(MockMigration.Object.TryGetDestinationApiEndpoint(out var endpoint));
                Assert.Same(destinationEndpoint, endpoint);
            }
        }
    }
}
