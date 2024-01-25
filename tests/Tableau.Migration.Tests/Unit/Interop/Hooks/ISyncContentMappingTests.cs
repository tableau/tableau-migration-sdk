using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Interop.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks
{
    public class ISyncContentMappingTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncMigrationActionCompletedHook
            {
                public virtual IMigrationActionResult? Execute(IMigrationActionResult ctx) => ctx;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var ctx = Create<IMigrationActionResult>();

                var result = await ((IMigrationHook<IMigrationActionResult>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockTransformer.Verify(x => x.Execute(ctx), Times.Once);
            }
        }
    }
}
