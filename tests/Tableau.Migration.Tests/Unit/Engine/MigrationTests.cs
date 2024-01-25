using System;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class MigrationTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var input = Create<IMigrationInput>();

                var mockPipelineFactory = Create<Mock<IMigrationPipelineFactory>>();

                var sourceEndpoint = Freeze<ISourceEndpoint>();
                var destinationEndpoint = Freeze<IDestinationEndpoint>();

                var mockEndpointFactory = Create<Mock<IMigrationEndpointFactory>>();

                var manifest = Create<IMigrationManifestEditor>();

                var mockManifestFactory = Freeze<Mock<IMigrationManifestFactory>>();
                mockManifestFactory.Setup(x => x.Create(input, It.IsAny<Guid>())).Returns(manifest);

                var m = new Migration.Engine.Migration(input, mockPipelineFactory.Object, mockEndpointFactory.Object, mockManifestFactory.Object);

                Assert.Equal(input.MigrationId, m.Id);
                Assert.Same(input.Plan, m.Plan);

                Assert.Same(sourceEndpoint, m.Source);
                Assert.Equal(destinationEndpoint, m.Destination);

                Assert.Same(manifest, m.Manifest);
                Assert.NotSame(input.PreviousManifest, m.Manifest);
            }
        }
    }
}
