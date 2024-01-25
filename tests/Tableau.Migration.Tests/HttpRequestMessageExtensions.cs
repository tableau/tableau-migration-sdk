using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests
{
    internal static class HttpRequestMessageExtensions
    {
        public static void AssertSingleHeaderValue(this HttpRequestMessage request, string headerKey, string headerValue)
        {
            request.AssertHeaderExists(headerKey);

            var actualHeaderValue = Assert.Single(request.Headers.GetValues(headerKey));

            Assert.Equal(headerValue, actualHeaderValue);
        }

        public static void AssertHeaderExists(this HttpRequestMessage request, string headerKey)
        {
            Assert.True(request.Headers.Contains(headerKey));
        }

        public static void AssertHeaderDoesNotExist(this HttpRequestMessage request, string headerKey)
        {
            Assert.False(request.Headers.Contains(headerKey));
        }

        public static void AssertUri(this HttpRequestMessage request, Uri expectedUri)
        {
            Assert.NotNull(request.RequestUri);
            Assert.Equal(expectedUri, new Uri(request.RequestUri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped)));
        }

        public static void AssertUri(this HttpRequestMessage request, Uri baseUri, string expectedRelativeUri)
        {
            AssertUri(request, new Uri(baseUri, expectedRelativeUri));
        }

        public static void AssertUri(
            this HttpRequestMessage request,
            TableauSiteConnectionConfiguration siteConnectionConfiguration,
            IServerSessionProvider sessionProvider,
            string expectedRelativeUri)
        {
            Assert.True(sessionProvider.Version.HasValue);

            request.AssertUri(
                new Uri(siteConnectionConfiguration.ServerUrl, $"/api/{sessionProvider.Version.Value.RestApiVersion}/"),
                expectedRelativeUri);
        }

        public static void AssertSiteUri(
            this HttpRequestMessage request,
            TableauSiteConnectionConfiguration siteConnectionConfiguration,
            IServerSessionProvider sessionProvider,
            string expectedRelativeUri)
        {
            Assert.True(sessionProvider.Version.HasValue);
            Assert.True(sessionProvider.SiteId.HasValue);

            request.AssertUri(
                new Uri(siteConnectionConfiguration.ServerUrl, $"/api/{sessionProvider.Version.Value.RestApiVersion}/sites/{sessionProvider.SiteId}/"),
                expectedRelativeUri);
        }

        public static void AssertSiteUri(
            this HttpRequestMessage request,
            TableauSiteConnectionConfiguration siteConnectionConfiguration,
            Mock<IServerSessionProvider> mockSessionProvider,
            string expectedRelativeUri)
            => request.AssertSiteUri(siteConnectionConfiguration, mockSessionProvider.Object, expectedRelativeUri);

        public static bool HasRelativeUri(this HttpRequestMessage request, string expectedRelativeUri)
        {
            if (request.RequestUri is null)
            {
                return false;
            }

            var actual = expectedRelativeUri.Split("/").Last().Contains('?')
                ? request.RequestUri.PathAndQuery
                : request.RequestUri.LocalPath;

            return string.Equals(expectedRelativeUri, actual);
        }

        public static void AssertRelativeUri(this HttpRequestMessage request, string expectedRelativeUri)
        {
            Assert.NotNull(request.RequestUri);

            var actual = expectedRelativeUri.Split("/").Last().Contains('?')
                ? request.RequestUri.PathAndQuery
                : request.RequestUri.LocalPath;

            Assert.Equal(expectedRelativeUri, actual);
        }

        public static async Task AssertContentAsync<TContent>(this HttpRequestMessage request, IHttpContentSerializer serializer, Action<TContent> assert)
        {
            Assert.NotNull(request.Content);

            var deserialized = await serializer.DeserializeAsync<TContent>(request.Content, default);

            Assert.NotNull(deserialized);

            assert(deserialized);
        }

        public static void AssertQuery(this HttpRequestMessage request, string expectedKey, string expectedValue)
        {
            Assert.NotNull(request.RequestUri);
            Assert.NotNull(request.RequestUri.Query);
            Assert.NotEmpty(request.RequestUri.Query);

            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            Assert.Contains(expectedKey, query.AllKeys);
            Assert.Equal(expectedValue, query[expectedKey]);
        }

        public static void AssertHttpMethod(this HttpRequestMessage request, HttpMethod method)
            => Assert.Equal(method, request.Method);
    }
}
