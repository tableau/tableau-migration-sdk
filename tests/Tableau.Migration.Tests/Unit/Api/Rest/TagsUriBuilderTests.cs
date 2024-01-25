using System;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class TagsUriBuilderTests
    {
        public abstract class TagsUriBuilderTest : AutoFixtureTestBase
        { }

        public class Prefix : TagsUriBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var prefix = Create<string>();

                var builder = new TagsUriBuilder(prefix);

                Assert.Equal(prefix, builder.Prefix);
            }
        }

        public class Suffix : TagsUriBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var builder = new TagsUriBuilder(Create<string>());

                Assert.Equal("tags", builder.Suffix);
            }
        }

        public class BuildUri : TagsUriBuilderTest
        {
            [Fact]
            public void Builds()
            {
                var prefix = Create<string>();
                var id = Create<Guid>();

                var builder = new TagsUriBuilder(prefix);

                var uri = builder.BuildUri(id);

                Assert.Equal(uri, $"{prefix}/{id.ToUrlSegment()}/tags");
            }
        }
    }
}
