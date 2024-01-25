using System;
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Config
{
    public class DefaultPermissionsContentTypeOptionsTests
    {
        public abstract class DefaultPermissionsContentTypeOptionsTest : AutoFixtureTestBase
        {
            protected static readonly IImmutableList<string> DefaultUrlSegments = DefaultPermissionsContentTypeUrlSegments.GetAll();
        }

        public class Ctor : DefaultPermissionsContentTypeOptionsTest
        {
            [Fact]
            public void Initializes_default()
            {
                var options = new DefaultPermissionsContentTypeOptions();

                Assert.True(options.UrlSegments.SequenceEqual(DefaultUrlSegments));
            }

            [Fact]
            public void Initializes_with_custom_segments()
            {
                var customUrlSegments = CreateMany<string>(10);

                var options = new DefaultPermissionsContentTypeOptions(customUrlSegments);

                Assert.True(options.UrlSegments.SequenceEqual(DefaultUrlSegments.Concat(customUrlSegments)));
            }

            [Fact]
            public void Deduplicates()
            {
                var options = new DefaultPermissionsContentTypeOptions(DefaultUrlSegments);

                Assert.True(options.UrlSegments.SequenceEqual(DefaultUrlSegments));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void Ignores_null_empty_or_whitespace_segments(string? customUrlSegment)
            {
                var options = new DefaultPermissionsContentTypeOptions(new[] { customUrlSegment! });

                Assert.True(options.UrlSegments.SequenceEqual(DefaultUrlSegments));
            }
        }
    }
}
