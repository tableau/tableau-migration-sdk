using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class DefaultHttpClientTests
    {
        public abstract class DefaultHttpClientTest : AutoFixtureTestBase
        {
            protected readonly Mock<HttpClient> MockHttpClient = new();
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();
            protected readonly Mock<IConfigReader> MockConfigReader = new();
            protected readonly Mock<HttpMessageHandler> MockHandler = new();

            protected readonly HttpContent DefaultContent = new MultipartContent();
            protected readonly MigrationSdkOptions migrationSdkOptions = new();

            public DefaultHttpClientTest()
            {
                MockConfigReader
                    .Setup(x => x.Get())
                    .Returns(migrationSdkOptions);

                MockHttpClient
                    .Setup(x =>
                        x.SendAsync(
                            It.IsAny<HttpRequestMessage>(),
                            Cancel))
                    .ReturnsAsync(new HttpResponseMessage());
            }

            internal DefaultHttpClient CreateClient(HttpClient innerClient) => new(innerClient, MockSerializer.Object);
            internal DefaultHttpClient CreateClient() => CreateClient(MockHttpClient.Object);
            internal DefaultHttpClient CreateClientWithHandler() => CreateClient(new HttpClient(MockHandler.Object));

            protected void SetupHandlerResponse(
                HttpResponseMessage response)
            {
                MockHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response);
            }

            internal async Task VerifySendAsyncCalled(
                Func<DefaultHttpClient, Task> execute,
                Expression<Func<HttpRequestMessage, bool>> assertRequest)
                => await VerifySendAsyncCalled(
                    execute,
                    assertRequest,
                    Times.Once());

            internal async Task VerifySendAsyncCalled(
                Func<DefaultHttpClient, Task> execute,
                Expression<Func<HttpRequestMessage, bool>> assertRequest,
                Times times)
            {
                SetupHandlerResponse(new HttpResponseMessage());

                var client = CreateClientWithHandler();

                await execute(client);

                MockHandler
                    .Protected()
                    .Verify(
                        "SendAsync",
                        times,
                        ItExpr.Is(assertRequest),
                        ItExpr.IsAny<CancellationToken>());
            }
        }

        #region - SendAsync -

        public class SendAsync : DefaultHttpClientTest
        {
            [Fact]
            public async Task Calls_inner_client_SendAsync()
            {
                await VerifySendAsyncCalled(
                    c => c.SendAsync(new HttpRequestMessage(HttpMethod.Options, TestConstants.LocalhostUri), Cancel),
                    r => r.Method == HttpMethod.Options);
            }
        }

        public class SendAsync_with_HttpCompletionOption : DefaultHttpClientTest
        {
            [Fact]
            public async Task Calls_inner_client_SendAsync()
            {
                await VerifySendAsyncCalled(
                    c => c.SendAsync(
                        new HttpRequestMessage(HttpMethod.Head, TestConstants.LocalhostUri),
                        HttpCompletionOption.ResponseHeadersRead, Cancel),
                    r => r.Method == HttpMethod.Head);
            }
        }

        public class SendAsyncServerInfo : DefaultHttpClientTest
        {
            [Fact]
            public async Task Calls_inner_client_SendAsync()
            {
                await VerifySendAsyncCalled(
                    c => c.SendAsync<ServerInfoResponse>(
                        new HttpRequestMessage(HttpMethod.Get, TestConstants.LocalhostUri),
                        Cancel),
                    r => r.Method == HttpMethod.Get);
            }
        }

        #endregion
    }
}
