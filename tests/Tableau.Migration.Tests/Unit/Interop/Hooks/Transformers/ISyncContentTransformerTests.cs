using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Interop.Hooks.Transformers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks.Transformers
{
    public class ISyncContentTransformerTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncContentTransformer<IUser>
            {
                public virtual IUser? Execute(IUser ctx) => ctx;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var ctx = Create<IUser>();

                var result = await ((IMigrationHook<IUser>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockTransformer.Verify(x => x.Execute(ctx), Times.Once);
            }
        }
    }
}
