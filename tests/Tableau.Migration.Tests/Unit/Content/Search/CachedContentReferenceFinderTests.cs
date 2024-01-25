using System;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Search
{
    public class CachedContentReferenceFinderTests
    {
        public class CachedContentReferenceFinderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IContentReferenceCache> MockCache;
            protected readonly CachedContentReferenceFinder<TestContentType> Finder;

            public CachedContentReferenceFinderTest()
            {
                MockCache = Freeze<Mock<IContentReferenceCache>>();
                Finder = Create<CachedContentReferenceFinder<TestContentType>>();
            }
        }

        public class FindByIdAsync : CachedContentReferenceFinderTest
        {
            [Fact]
            public async Task CallsCacheAsync()
            {
                var id = Guid.NewGuid();

                var cacheResult = Create<IContentReference>();
                MockCache.Setup(x => x.ForIdAsync(id, Cancel))
                    .ReturnsAsync(cacheResult);

                var result = await Finder.FindByIdAsync(id, Cancel);

                Assert.Same(cacheResult, result);
                MockCache.Verify(x => x.ForIdAsync(id, Cancel), Times.Once);
            }
        }
    }
}
