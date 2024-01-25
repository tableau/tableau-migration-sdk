using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class IHttpRequestBuilderExtensionsTests
    {
        public abstract class IHttpRequestBuilderExtensionsTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpRequestBuilder> MockRequestBuilder = new();

            protected readonly Mock<IHttpResponseMessage> MockResponse = new();
            protected readonly Mock<HttpHeaders> MockHeaders = new();

            public IHttpRequestBuilderExtensionsTest()
            {
                MockRequestBuilder.Setup(x => x.SendAsync(HttpCompletionOption.ResponseHeadersRead, Cancel))
                    .ReturnsAsync(MockResponse.Object);

                MockResponse.SetupGet(x => x.Headers).Returns(MockHeaders.Object);
            }
        }

        public class DownloadAsync : IHttpRequestBuilderExtensionsTest
        {
            [Fact]
            public async Task SendsAndDownloadsAsync()
            {
                var content = new ByteArrayContent(Encoding.UTF8.GetBytes("test content"));
                MockResponse.Setup(x => x.Content).Returns(content);

                var result = await MockRequestBuilder.Object
                    .DownloadAsync(Cancel);

                MockRequestBuilder.Verify(x => x.SendAsync(HttpCompletionOption.ResponseHeadersRead, Cancel), Times.Once);

                result.AssertSuccess();
            }
        }
    }
}
