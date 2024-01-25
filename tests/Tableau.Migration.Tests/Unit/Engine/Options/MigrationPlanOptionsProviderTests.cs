using System;
using Microsoft.Extensions.Options;
using Moq;
using Tableau.Migration.Engine.Options;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Options
{
    public class MigrationPlanOptionsProviderTests
    {
        public class Get : AutoFixtureTestBase
        {
            private readonly MockServiceProvider _mockServices;
            private readonly Mock<IMigrationPlanOptionsCollection> _mockPlanOptions;

            private readonly MigrationPlanOptionsProvider<TestPlanOptions> _provider;

            public Get()
            {
                _mockServices = new(AutoFixture);
                _mockServices.Setup(x => x.GetService(typeof(TestPlanOptions)))
                    .Returns(null);
                _mockServices.Setup(x => x.GetService(typeof(IOptions<TestPlanOptions>)))
                    .Returns(null);

                _mockPlanOptions = Freeze<Mock<IMigrationPlanOptionsCollection>>();
                _mockPlanOptions.Setup(x => x.Get<It.IsAnyType>(It.IsAny<IServiceProvider>()))
                    .Returns((It.IsAnyType?)null);

                var plan = Create<IMigrationPlan>();

                _provider = new(_mockServices.Object, plan);
            }

            [Fact]
            public void PrefersPlanOptions()
            {
                var opts = new TestPlanOptions { TestOption = 12 };

                _mockPlanOptions.Setup(x => x.Get<TestPlanOptions>(_mockServices.Object))
                    .Returns(opts);

                var result = _provider.Get();

                Assert.Same(opts, result);

                _mockPlanOptions.Verify(x => x.Get<TestPlanOptions>(_mockServices.Object), Times.Once);
                _mockServices.Verify(x => x.GetService(typeof(TestPlanOptions)), Times.Never);
                _mockServices.Verify(x => x.GetService(typeof(IOptions<TestPlanOptions>)), Times.Never);
            }

            [Fact]
            public void FallsBackToDependencyInjection()
            {
                var opts = new TestPlanOptions { TestOption = 12 };
                _mockServices.Setup(x => x.GetService(typeof(TestPlanOptions)))
                    .Returns(opts);

                var result = _provider.Get();

                Assert.Same(opts, result);

                _mockPlanOptions.Verify(x => x.Get<TestPlanOptions>(_mockServices.Object), Times.Once);
                _mockServices.Verify(x => x.GetService(typeof(TestPlanOptions)), Times.Once);
                _mockServices.Verify(x => x.GetService(typeof(IOptions<TestPlanOptions>)), Times.Never);
            }

            [Fact]
            public void FallsBackToDependencyInjectionIOptions()
            {
                var opts = new TestPlanOptions { TestOption = 12 };
                var mockOpts = new Mock<IOptions<TestPlanOptions>>();
                mockOpts.Setup(x => x.Value)
                    .Returns(opts);
                _mockServices.Setup(x => x.GetService(typeof(IOptions<TestPlanOptions>)))
                    .Returns(mockOpts.Object);

                var result = _provider.Get();

                Assert.Same(opts, result);

                _mockPlanOptions.Verify(x => x.Get<TestPlanOptions>(_mockServices.Object), Times.Once);
                _mockServices.Verify(x => x.GetService(typeof(TestPlanOptions)), Times.Once);
                _mockServices.Verify(x => x.GetService(typeof(IOptions<TestPlanOptions>)), Times.Once);
            }

            [Fact]
            public void FallsBackToDefaultConstructor()
            {
                var result = _provider.Get();

                Assert.Equal(new TestPlanOptions().TestOption, result.TestOption);

                _mockPlanOptions.Verify(x => x.Get<TestPlanOptions>(_mockServices.Object), Times.Once);
                _mockServices.Verify(x => x.GetService(typeof(TestPlanOptions)), Times.Once);
                _mockServices.Verify(x => x.GetService(typeof(IOptions<TestPlanOptions>)), Times.Once);
            }
        }
    }
}
