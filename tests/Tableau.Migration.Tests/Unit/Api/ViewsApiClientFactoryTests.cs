using Tableau.Migration.Api;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ViewsApiClientFactoryTests
    {
        public class Create : AutoFixtureTestBase
        {
            [Fact]
            public void CreatesViewsClient()
            {
                var fact = Create<ViewsApiClientFactory>();

                var viewsClient = fact.Create();

                Assert.IsType<ViewsApiClient>(viewsClient);
            }
        }
    }
}
