using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish.Default
{
    public class TagItemPostPublishHookTests
    {
        public class TagContentType : TestContentType, IWithTags
        {
            public IList<ITag> Tags { get; set; } = new List<ITag>();
        }

        public class TagItemPostPublishHookTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration = new();
            protected readonly Mock<IDestinationEndpoint> MockDestinationEndpoint = new();

            protected readonly TagItemPostPublishHook<TagContentType, TestContentType> Hook;

            public TagItemPostPublishHookTest()
            {
                MockMigration.SetupGet(m => m.Destination).Returns(MockDestinationEndpoint.Object);

                Hook = new(MockMigration.Object);
            }
        }

        public class ExecuteAsync : TagItemPostPublishHookTest
        {
            [Fact]
            public async Task Succeeds()
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();
                var sourceItem = Create<TagContentType>();
                var destinationItem = Create<TagContentType>();

                var context = new ContentItemPostPublishContext<TagContentType, TestContentType>(manifestEntry, sourceItem, destinationItem);

                var updateResult = Result.Succeeded();

                MockDestinationEndpoint.Setup(e => e.UpdateTagsAsync<TagContentType>(destinationItem, sourceItem.Tags, Cancel))
                    .ReturnsAsync(updateResult);

                var result = await Hook.ExecuteAsync(context, Cancel);

                Assert.Same(context, result);

                MockDestinationEndpoint.VerifyAll();
            }
        }
    }
}
