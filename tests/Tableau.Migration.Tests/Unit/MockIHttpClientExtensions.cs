using System;
using System.Net.Http;
using System.Threading;
using Moq;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Unit
{
    public static class MockIHttpClientExtensions
    {
        public static void SetupResponse<TResponse>(
            this Mock<IHttpClient> mockHttpClient,
            IHttpResponseMessage<TResponse> response,
            Action<HttpRequestMessage>? onRequestSent = null)
            where TResponse : class
        {
            var returns = mockHttpClient
                .Setup(c => c.SendAsync<TResponse>(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            if (onRequestSent is not null)
                returns.Callback((HttpRequestMessage request, CancellationToken cancel) => onRequestSent.Invoke(request));
        }

        public static void SetupResponse(
            this Mock<IHttpClient> mockHttpClient,
            IHttpResponseMessage response,
            Action<HttpRequestMessage>? onRequestSent = null)
        {
            var returns = mockHttpClient
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            if (onRequestSent is not null)
                returns.Callback((HttpRequestMessage request, CancellationToken cancel) => onRequestSent.Invoke(request));

            returns = mockHttpClient
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            if (onRequestSent is not null)
                returns.Callback((HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancel) => onRequestSent.Invoke(request));

        }
    }
}
