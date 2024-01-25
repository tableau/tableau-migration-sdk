using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Net.Handlers;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Handlers
{
    public class AuthenticationHandlerTests
    {
        public abstract class AuthenticationHandlerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IAuthenticationTokenProvider> MockTokenProvider = new();

            internal readonly AuthenticationHandler Handler;

            public AuthenticationHandlerTest()
            {
                Handler = new(MockTokenProvider.Object);
            }

            protected static MockDelegatingHandler CreateMockDelegatingHandler(
                HttpStatusCode responseStatusCode,
                Action<HttpRequestMessage, int>? assertRequest = null)
            {
                var count = 0;

                return new MockDelegatingHandler(request =>
                {
                    count++;

                    assertRequest?.Invoke(request, count);

                    return new HttpResponseMessage(responseStatusCode);
                });
            }

            protected static void AssertAuthenticationHeader(HttpRequestMessage request, string? expectedToken)
            {
                if (expectedToken is null)
                {
                    request.AssertHeaderDoesNotExist(RestHeaders.AuthenticationToken);
                }
                else
                {
                    request.AssertSingleHeaderValue(RestHeaders.AuthenticationToken, expectedToken);
                }
            }
        }

        public class SendAsync : AuthenticationHandlerTest
        {
            [Fact]
            public async Task Skips_if_not_Rest_request()
            {
                var mockHandler = CreateMockDelegatingHandler(
                    HttpStatusCode.OK,
                    (request, _) =>
                    {
                        AssertAuthenticationHeader(request, null);
                    });

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/not-api/3.20/test"), Cancel);

                MockTokenProvider.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task Sets_token()
            {
                var token = Create<string>();

                MockTokenProvider.SetupGet(p => p.Token).Returns(token);

                var mockHandler = CreateMockDelegatingHandler(
                    HttpStatusCode.OK,
                    (request, _) =>
                    {
                        AssertAuthenticationHeader(request, token);
                    });

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/3.20/test"), Cancel);

                MockTokenProvider.Verify(p => p.RequestRefreshAsync(It.IsAny<CancellationToken>()), Times.Never());
            }

            [Fact]
            public async Task Overwrites_token_with_provider_value()
            {
                var token = Create<string>();

                MockTokenProvider.SetupGet(p => p.Token).Returns(token);

                var mockHandler = CreateMockDelegatingHandler(
                    HttpStatusCode.OK,
                    (request, _) =>
                    {
                        AssertAuthenticationHeader(request, token);
                    });

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/3.20/test");
                request.Headers.TryAddWithoutValidation(RestHeaders.AuthenticationToken, Create<string>());

                await invoker.SendAsync(request, Cancel);
            }

            [Fact]
            public async Task Refreshes_token()
            {
                var token1 = Create<string>();
                var token2 = Create<string>();

                MockTokenProvider.SetupGet(p => p.Token).Returns(token1);

                MockTokenProvider
                    .Setup(p => p.RequestRefreshAsync(Cancel))
                    .Callback(() => MockTokenProvider.SetupGet(p => p.Token).Returns(token2));

                var mockHandler = CreateMockDelegatingHandler(
                    HttpStatusCode.Unauthorized,
                    (request, count) =>
                    {
                        AssertAuthenticationHeader(request, count == 1 ? token1 : token2);
                    });

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/3.20/test"), Cancel);

                MockTokenProvider.Verify(p => p.RequestRefreshAsync(It.IsAny<CancellationToken>()), Times.Once());
            }

            [Fact]
            public async Task Does_not_set_token_on_sign_in_attempts()
            {
                var mockHandler = CreateMockDelegatingHandler(HttpStatusCode.Unauthorized);

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/3.20/auth/signin"), Cancel);

                MockTokenProvider.VerifyNoOtherCalls();
            }
        }
    }
}
