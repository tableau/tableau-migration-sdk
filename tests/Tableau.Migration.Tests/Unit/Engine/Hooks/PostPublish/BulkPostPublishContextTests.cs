using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish
{
    public class BulkPostPublishContextTests
    {
        public abstract class BulkPostPublishContextTest : AutoFixtureTestBase
        { }

        public class Ctor : BulkPostPublishContextTest
        {
            [Fact]
            public void Initializes()
            {
                var sourceItems = CreateMany<TestContentType>(5);

                var ctx = new BulkPostPublishContext<TestContentType>(sourceItems);

                Assert.True(sourceItems.SequenceEqual(ctx.PublishedItems));
            }
        }

        public class ToTask : BulkPostPublishContextTest
        {
            [Fact]
            public async Task CreatesCompletedTask()
            {
                var sourceItems = CreateMany<TestContentType>(5);

                var ctx = new BulkPostPublishContext<TestContentType>(sourceItems);

                var task = ctx.ToTask();

                var result = await task;

                Assert.True(task.IsCompleted);
                Assert.Same(ctx, result);
            }
        }
    }
}
