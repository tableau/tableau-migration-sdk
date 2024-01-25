using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Preparation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public class EndpointContentItemPreparerTests
    {
        public class PullAsync : ContentItemPreparerTestBase<TestPublishType>
        {
            private readonly Mock<ISourceEndpoint> _mockSourceEndpoint;
            private readonly EndpointContentItemPreparer<TestContentType, TestPublishType> _preparer;

            public PullAsync()
            {
                _mockSourceEndpoint = Freeze<Mock<ISourceEndpoint>>();
                _preparer = Create<EndpointContentItemPreparer<TestContentType, TestPublishType>>();
            }

            [Fact]
            public async Task PullsFromSourceEndpointAsync()
            {
                var pullResult = Result<TestPublishType>.Succeeded(new());
                _mockSourceEndpoint.Setup(x => x.PullAsync<TestContentType, TestPublishType>(Item.SourceItem, Cancel))
                    .ReturnsAsync(pullResult);

                var result = await _preparer.PrepareAsync(Item, Cancel);

                result.AssertSuccess();
                Assert.Same(pullResult.Value, result.Value);
                _mockSourceEndpoint.Verify(x => x.PullAsync<TestContentType, TestPublishType>(Item.SourceItem, Cancel), Times.Once);
            }
        }
    }
}
