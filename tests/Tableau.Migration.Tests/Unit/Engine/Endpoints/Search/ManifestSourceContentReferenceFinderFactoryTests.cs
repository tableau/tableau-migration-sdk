using Moq;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class ManifestSourceContentReferenceFinderFactoryTests
    {
        public class ForContentType : AutoFixtureTestBase
        {
            [Fact]
            public void ReturnsManifestSourceFinder()
            {
                var provider = Freeze<MockServiceProvider>();

                var fac = Create<ManifestSourceContentReferenceFinderFactory>();

                var finder = fac.ForContentType<TestContentType>();

                provider.Verify(x => x.GetService(typeof(ManifestSourceContentReferenceFinder<TestContentType>)), Times.Once);
            }
        }
    }
}
