using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Interop.Hooks.Filters;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks.Filters
{
    public class ISyncContentFilterTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncContentFilter<IUser>
            {
                public virtual IEnumerable<ContentMigrationItem<IUser>>? Execute(IEnumerable<ContentMigrationItem<IUser>> ctx) => ctx;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var ctx = Create<IEnumerable<ContentMigrationItem<IUser>>>();

                var result = await ((IMigrationHook<IEnumerable<ContentMigrationItem<IUser>>>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockTransformer.Verify(x => x.Execute(ctx), Times.Once);
            }
        }
    }
}
