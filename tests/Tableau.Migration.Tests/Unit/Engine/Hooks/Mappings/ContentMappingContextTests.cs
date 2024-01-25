using System.Threading.Tasks;
using Tableau.Migration.Engine.Hooks.Mappings;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings
{
    public class ContentMappingContextTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var item = Create<TestContentType>();
                var mappedLocation = Create<ContentLocation>();

                var ctx = new ContentMappingContext<TestContentType>(item, mappedLocation);

                Assert.Same(item, ctx.ContentItem);
                Assert.Equal(mappedLocation, ctx.MappedLocation);
            }
        }

        public class MapTo : AutoFixtureTestBase
        {
            [Fact]
            public void MapsLocation()
            {
                var item = Create<TestContentType>();
                var mappedLocation = Create<ContentLocation>();

                var ctx1 = new ContentMappingContext<TestContentType>(item, item.Location);
                var ctx2 = ctx1.MapTo(mappedLocation);

                Assert.NotSame(ctx1, ctx2);
                Assert.Same(ctx1.ContentItem, ctx2.ContentItem);
                Assert.Equal(mappedLocation, ctx2.MappedLocation);
            }
        }

        public class ToTask : AutoFixtureTestBase
        {
            [Fact]
            public async Task CreatesCompletedTaskAsync()
            {
                var item = Create<TestContentType>();
                var ctx = new ContentMappingContext<TestContentType>(item, item.Location);

                var taskContext = await ctx.ToTask();

                Assert.Same(ctx, taskContext);
            }
        }
    }
}
