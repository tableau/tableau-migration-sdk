using System.Threading.Tasks;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish
{
    public class ContentItemPostPublishContextTests
    {
        public abstract class ContentItemPostPublishContextTest : AutoFixtureTestBase
        { }

        public class SameSourceAndDestinationType
        {
            public class Ctor : ContentItemPostPublishContextTest
            {
                [Fact]
                public void Initializes()
                {
                    var manifestEntry = Create<IMigrationManifestEntryEditor>();
                    var publish = Create<TestPublishType>();
                    var result = Create<TestContentType>();

                    var ctx = new ContentItemPostPublishContext<TestPublishType, TestContentType>(manifestEntry, publish, result);

                    Assert.Same(manifestEntry, ctx.ManifestEntry);
                    Assert.Same(publish, ctx.PublishedItem);
                    Assert.Same(result, ctx.DestinationItem);
                }
            }

            public class ToTask : ContentItemPostPublishContextTest
            {
                [Fact]
                public async Task CreatesCompletedTask()
                {
                    var ctx = Create<ContentItemPostPublishContext<TestPublishType, TestContentType>>();

                    var task = ctx.ToTask();

                    var result = await task;

                    Assert.True(task.IsCompleted);
                    Assert.Same(ctx, result);
                }
            }
        }

        public class DifferentSourceAndDestinationTypes
        {
            private class SourceContentType : TestContentType
            { }

            private class DestinationContentType : TestContentType
            { }

            public class Ctor : ContentItemPostPublishContextTest
            {
                [Fact]
                public void Initializes()
                {
                    var manifestEntry = Create<IMigrationManifestEntryEditor>();
                    var source = Create<SourceContentType>();
                    var destination = Create<DestinationContentType>();

                    var ctx = new ContentItemPostPublishContext<SourceContentType, DestinationContentType>(manifestEntry, source, destination);

                    Assert.Same(manifestEntry, ctx.ManifestEntry);
                    Assert.Same(source, ctx.PublishedItem);
                    Assert.Same(destination, ctx.DestinationItem);
                }
            }

            public class ToTask : ContentItemPostPublishContextTest
            {
                [Fact]
                public async Task CreatesCompletedTask()
                {
                    var ctx = Create<ContentItemPostPublishContext<SourceContentType, DestinationContentType>>();

                    var task = ctx.ToTask();

                    var result = await task;

                    Assert.True(task.IsCompleted);
                    Assert.Same(ctx, result);
                }
            }
        }
    }
}
