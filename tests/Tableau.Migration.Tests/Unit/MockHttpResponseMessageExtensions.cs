using System;
using System.Net;
using System.Net.Http;
using Moq;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Unit
{
    public static class MockHttpResponseMessageExtensions
    {
        public static TMockResponseMessage WithStatusCode<TMockResponseMessage, TResponseMessage>(
            this TMockResponseMessage mockResponse,
            HttpStatusCode statusCode)
            where TMockResponseMessage : Mock<TResponseMessage>
            where TResponseMessage : class, IHttpResponseMessage
        {
            mockResponse.SetupGet(r => r.StatusCode).Returns(statusCode);
            mockResponse.SetupGet(r => r.IsSuccessStatusCode).Returns(() => mockResponse.IsSuccessStatusCode());
            mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Returns(() => mockResponse.EnsureSuccessStatusCode());

            return mockResponse;
        }

        public static TMockResponseMessage WithSuccessStatusCode<TMockResponseMessage, TResponseMessage>(
            this TMockResponseMessage mockResponse)
            where TMockResponseMessage : Mock<TResponseMessage>
            where TResponseMessage : class, IHttpResponseMessage
            => mockResponse.WithStatusCode<TMockResponseMessage, TResponseMessage>(HttpStatusCode.OK);

        public static TMockResponseMessage WithContent<TMockResponseMessage, TResponseMessage>(
            this TMockResponseMessage mockResponse,
            HttpContent? content)
            where TMockResponseMessage : Mock<TResponseMessage>
            where TResponseMessage : class, IHttpResponseMessage
        {
            if (content is not null)
                mockResponse.SetupGet(r => r.Content).Returns(content);

            return mockResponse;
        }

        public static TMockResponseMessage WithEnsureSuccessStatusCodeException<TMockResponseMessage, TResponseMessage>(
            this TMockResponseMessage mockResponse,
            Exception exception)
            where TMockResponseMessage : Mock<TResponseMessage>
            where TResponseMessage : class, IHttpResponseMessage
        {
            mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

            return mockResponse;
        }

        public static TMockResponseMessage WithDeserializedContent<TMockResponseMessage, TResponseMessage, TContent>(
            this TMockResponseMessage mockResponse,
            TContent? content)
            where TMockResponseMessage : Mock<TResponseMessage>
            where TResponseMessage : class, IHttpResponseMessage<TContent>
            where TContent : class
        {
            mockResponse.SetupGet(r => r.DeserializedContent).Returns(content);

            return mockResponse;
        }
    }
}
