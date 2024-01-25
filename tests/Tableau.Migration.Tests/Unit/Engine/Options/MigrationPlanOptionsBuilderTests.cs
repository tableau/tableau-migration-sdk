using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Engine.Options;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Options
{
    public class MigrationPlanOptionsBuilderTests
    {
        public class Roundtrip : AutoFixtureTestBase
        {
            private readonly MockServiceProvider _mockServices;
            private readonly MigrationPlanOptionsBuilder _builder;

            public Roundtrip()
            {
                _mockServices = new(AutoFixture);
                _builder = new();
            }

            [Fact]
            public void FromObject()
            {
                var opts = new TestPlanOptions { TestOption = 144 };

                var result = _builder.Configure(opts);

                Assert.Same(result, _builder);
                Assert.Same(opts, _builder.Build().Get<TestPlanOptions>(_mockServices.Object));
            }

            [Fact]
            public void FromDependencyInjection()
            {
                var opts = new TestPlanOptions { TestOption = 144 };
                _mockServices.Setup(x => x.GetService(typeof(TestPlanOptions))).Returns(opts);
                var optsFactory = (IServiceProvider s) => s.GetRequiredService<TestPlanOptions>();

                var result = _builder.Configure(optsFactory);

                Assert.Same(result, _builder);
                Assert.Same(opts, _builder.Build().Get<TestPlanOptions>(_mockServices.Object));
                _mockServices.Verify(x => x.GetService(typeof(TestPlanOptions)), Times.Once);
            }
        }
    }
}
