using Moq;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Content.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Search
{
    public class ApiContentReferenceFinderFactoryTests
    {
        public class ForContentType : AutoFixtureTestBase
        {
            [Fact]
            public void BuildsCachedApiFinder()
            {
                var cache = Freeze<BulkApiContentReferenceCache<TestContentType>>();
                var mockServices = Freeze<MockServiceProvider>();

                var factory = Create<ApiContentReferenceFinderFactory>();

                var finder = factory.ForContentType<TestContentType>();

                Assert.IsType<CachedContentReferenceFinder<TestContentType>>(finder);
                mockServices.Verify(x => x.GetService(typeof(BulkApiContentReferenceCache<TestContentType>)), Times.Once);
            }
        }
    }
}
