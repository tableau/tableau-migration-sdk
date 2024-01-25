using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Interop.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks
{
    public class ISyncContentTransformerTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncMigrationHook<string>
            {
                public virtual string? Execute(string ctx) => ctx;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var ctx = Create<string>();

                var result = await ((IMigrationHook<string>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockTransformer.Verify(x => x.Execute(ctx), Times.Once);
            }
        }
    }
}
