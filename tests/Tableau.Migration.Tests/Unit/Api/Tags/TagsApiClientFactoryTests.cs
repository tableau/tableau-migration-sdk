using Tableau.Migration.Api;
using Tableau.Migration.Api.Tags;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Tags
{
    public class TagsApiClientFactoryTests
    {
        public class Create : AutoFixtureTestBase
        {
            [Fact]
            public void CreatesTagsClient()
            {
                var fact = Create<TagsApiClientFactory>();

                var apiClient = Create<IContentApiClient>();

                var tagsClient = fact.Create(apiClient);

                Assert.IsType<TagsApiClient>(tagsClient);
            }
        }
    }
}
