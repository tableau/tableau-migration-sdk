using Tableau.Migration.Api;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class TableauServerVersionProviderTests
    {
        public abstract class TableauServerVersionProviderTest : AutoFixtureTestBase
        {
            internal readonly TableauServerVersionProvider Provider = new();
        }

        public class Set : TableauServerVersionProviderTest
        {
            [Fact]
            public void Sets_version()
            {
                var version = Create<TableauServerVersion>();

                Provider.Set(version);

                var providerVersion = Assert.NotNull(Provider.Version);
                Assert.Equal(version, providerVersion);
            }
        }
    }
}
