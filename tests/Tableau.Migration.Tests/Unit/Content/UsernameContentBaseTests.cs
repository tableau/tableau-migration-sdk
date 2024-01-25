using System.Collections.Immutable;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class UsernameContentBaseTests
    {
        public class TestUsernameContent : UsernameContentBase
        {
            new public string Name
            {
                get => base.Name;
                set => base.Name = value;
            }

            new public string Domain
            {
                get => base.Domain;
                set => base.Domain = value;
            }
        }

        public class Name : AutoFixtureTestBase
        {
            [Fact]
            public void UpdatesLocation()
            {
                var c = new TestUsernameContent();

                c.Name = Create<string>();

                Assert.Equal(ContentLocation.ForUsername(c.Domain, c.Name), c.Location);
            }
        }

        public class Domain : AutoFixtureTestBase
        {
            [Fact]
            public void UpdatesLocation()
            {
                var c = new TestUsernameContent();

                c.Domain = Create<string>();

                Assert.Equal(ContentLocation.ForUsername(c.Domain, c.Name), c.Location);
            }
        }

        public class SetLocation
        {
            [Theory]
            [InlineData(new string[0], "", "")]
            [InlineData(new[] { "username" }, "", "username")]
            [InlineData(new[] { "domain", "username" }, "domain", "username")]
            [InlineData(new[] { "domain", "username", "extra" }, "domain", "username")]
            public void SetsDomainAndUsername(string[] segments, string expectedDomain, string expectedUsername)
            {
                var c = new TestUsernameContent();

                var mappedLoc = new ContentLocation(segments.ToImmutableArray(), Constants.DomainNameSeparator);
                ((IMappableContent)c).SetLocation(mappedLoc);

                Assert.Equal(expectedDomain, c.Domain);
                Assert.Equal(expectedUsername, c.Name);
            }
        }
    }
}
