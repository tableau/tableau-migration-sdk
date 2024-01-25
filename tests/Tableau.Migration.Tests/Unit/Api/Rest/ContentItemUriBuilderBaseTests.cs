using System;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class ContentItemUriBuilderBaseTests
    {
        private class TestUriBuilder : ContentItemUriBuilderBase
        {
            public TestUriBuilder(string prefix, string suffix)
                : base(prefix, suffix)
            { }
        }

        public abstract class ContentItemUriBuilderBaseTest : AutoFixtureTestBase
        { }

        public class Ctor : ContentItemUriBuilderBaseTest
        {
            [Fact]
            public void Initializes()
            {
                var prefix = Create<string>();
                var suffix = Create<string>();

                var builder = new TestUriBuilder(prefix, suffix);

                Assert.Equal(prefix, builder.Prefix);
                Assert.Equal(suffix, builder.Suffix);
            }
        }

        public class BuildUri : ContentItemUriBuilderBaseTest
        {
            [Fact]
            public void Builds()
            {
                var prefix = Create<string>();
                var suffix = Create<string>();
                var id = Create<Guid>();

                var builder = new TestUriBuilder(prefix, suffix);

                var uri = builder.BuildUri(id);

                Assert.Equal(uri, $"{prefix}/{id.ToUrlSegment()}/{suffix}");
            }
        }
    }
}
