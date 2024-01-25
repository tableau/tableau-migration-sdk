using Moq;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class ManifestDestinationContentReferenceFinderFactoryTests
    {
        public class ForContentType : AutoFixtureTestBase
        {
            [Fact]
            public void ReturnsManifestDestinationFinder()
            {
                var provider = Freeze<MockServiceProvider>();

                var fac = Create<ManifestDestinationContentReferenceFinderFactory>();

                var finder = fac.ForContentType<TestContentType>();

                provider.Verify(x => x.GetService(typeof(ManifestDestinationContentReferenceFinder<TestContentType>)), Times.Once);
            }
        }
    }
}
