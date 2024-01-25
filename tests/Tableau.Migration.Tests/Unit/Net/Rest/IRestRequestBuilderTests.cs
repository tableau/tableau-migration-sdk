using Moq;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest
{
    public class IRestRequestBuilderTests
    {
        public class WithPage
        {
            [Fact]
            public void PageNumberSizeComponentOverride()
            {
                var mockBuilder = new Mock<IRestRequestBuilder> { CallBase = true };
                mockBuilder.Setup(x => x.WithPage(It.IsAny<Page>())).Returns(mockBuilder.Object);

                var result = mockBuilder.Object.WithPage(2, 4);

                Assert.Same(mockBuilder.Object, result);
                mockBuilder.Verify(x => x.WithPage(It.Is<Page>(p => p == new Page(2, 4))), Times.Once);
            }
        }
    }
}
