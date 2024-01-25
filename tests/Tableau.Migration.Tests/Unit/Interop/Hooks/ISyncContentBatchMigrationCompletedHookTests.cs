using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Interop.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks
{
    public class ISyncContentBatchMigrationCompletedHookTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncContentBatchMigrationCompletedHook<IUser>
            {
                public virtual IContentBatchMigrationResult<IUser>? Execute(IContentBatchMigrationResult<IUser> ctx) => ctx;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var ctx = Create<IContentBatchMigrationResult<IUser>>();

                var result = await ((IMigrationHook<IContentBatchMigrationResult<IUser>>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockTransformer.Verify(x => x.Execute(ctx), Times.Once);
            }
        }
    }
}
