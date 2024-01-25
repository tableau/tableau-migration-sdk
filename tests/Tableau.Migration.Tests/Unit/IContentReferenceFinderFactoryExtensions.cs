using AutoFixture;
using Moq;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Tests.Unit
{
    public static class IContentReferenceFinderFactoryExtensions
    {
        public static Mock<IContentReferenceFinder<TContent>> SetupMockFinder<TContent>(
            this Mock<IContentReferenceFinderFactory> mockFinderFactory,
            IFixture autoFixture)
            where TContent : IContentReference
        {
            var mockFinder = autoFixture.Create<Mock<IContentReferenceFinder<TContent>>>();
            mockFinderFactory.Setup(x => x.ForContentType<TContent>()).Returns(mockFinder.Object);

            return mockFinder;
        }
    }
}
