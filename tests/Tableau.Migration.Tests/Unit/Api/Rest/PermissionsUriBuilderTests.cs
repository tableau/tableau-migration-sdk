using System;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class PermissionsUriBuilderTests
    {
        public abstract class PermissionsUriBuilderTest : AutoFixtureTestBase
        { }

        public class Prefix : PermissionsUriBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var prefix = Create<string>();

                var builder = new PermissionsUriBuilder(prefix, Create<string>());

                Assert.Equal(prefix, builder.Prefix);
            }
        }

        public class Suffix : PermissionsUriBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var suffix = Create<string>();

                var builder = new PermissionsUriBuilder(Create<string>(), suffix);

                Assert.Equal(suffix, builder.Suffix);
            }
        }

        public class BuildUri : PermissionsUriBuilderTest
        {
            [Fact]
            public void Builds()
            {
                var prefix = Create<string>();
                var suffix = Create<string>();
                var id = Create<Guid>();

                var builder = new PermissionsUriBuilder(prefix, suffix);

                var uri = builder.BuildUri(id);

                Assert.Equal(uri, $"{prefix}/{id.ToUrlSegment()}/{suffix}");
            }
        }

        public class BuildDeleteUri : PermissionsUriBuilderTest
        {
            [Fact]
            public void Builds()
            {
                var prefix = Create<string>();
                var suffix = Create<string>();
                var id = Create<Guid>();
                var capability = Create<ICapability>();
                var granteeType = Create<GranteeType>();
                var granteeId = Create<Guid>();

                var builder = new PermissionsUriBuilder(prefix, suffix);

                var uri = builder.BuildDeleteUri(id, capability, granteeType, granteeId);

                Assert.Equal(uri, $"{prefix}/{id.ToUrlSegment()}/{suffix}/{granteeType.ToUrlSegment()}/{granteeId.ToUrlSegment()}/{capability.Name}/{capability.Mode}");
            }
        }
    }
}
