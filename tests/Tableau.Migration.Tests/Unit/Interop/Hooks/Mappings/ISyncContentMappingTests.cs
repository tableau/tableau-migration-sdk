using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Interop.Hooks.Mappings;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks.Mappings
{
    public class ISyncContentMappingTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncContentMapping<IUser>
            {
                public virtual ContentMappingContext<IUser>? Execute(ContentMappingContext<IUser> ctx) => ctx;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var ctx = Create<ContentMappingContext<IUser>>();

                var result = await ((IMigrationHook<ContentMappingContext<IUser>>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockTransformer.Verify(x => x.Execute(ctx), Times.Once);
            }
        }
    }
}
