using System;
using Moq;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineFactoryTests
    {
        public class Create : AutoFixtureTestBase
        {
            private readonly MockServiceProvider _mockServices;

            private readonly MigrationPipelineFactory _factory;

            public Create()
            {
                _mockServices = Create<MockServiceProvider>();

                _factory = new(_mockServices.Object);
            }

            [Fact]
            public void CreatesServerToCloudMigration()
            {
                var pipeline = new ServerToCloudMigrationPipeline(_mockServices.Object);
                _mockServices.Setup(x => x.GetService(typeof(ServerToCloudMigrationPipeline))).Returns(pipeline);

                var mockPlan = Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.PipelineProfile).Returns(PipelineProfile.ServerToCloud);

                var result = _factory.Create(mockPlan.Object);

                Assert.Same(pipeline, result);
            }

            [Fact]
            public void UnsupportedProfile()
            {
                var mockPlan = Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.PipelineProfile).Returns((PipelineProfile)int.MaxValue);

                Assert.Throws<ArgumentException>(() => _factory.Create(mockPlan.Object));
            }
        }
    }
}
