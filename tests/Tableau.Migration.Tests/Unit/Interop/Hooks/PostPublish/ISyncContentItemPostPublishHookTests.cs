using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Interop.Hooks.PostPublish;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks.PostPublish
{
    public class ISyncContentItemPostPublishHookTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncContentItemPostPublishHook<IUser, IUser>
            {
                public virtual ContentItemPostPublishContext<IUser, IUser>? Execute(ContentItemPostPublishContext<IUser, IUser> ctx) => ctx;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var ctx = Create<ContentItemPostPublishContext<IUser, IUser>>();

                var result = await ((IMigrationHook<ContentItemPostPublishContext<IUser, IUser>>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockTransformer.Verify(x => x.Execute(ctx), Times.Once);
            }
        }
    }
}
