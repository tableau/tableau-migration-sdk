using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class SitesApiClientTests
    {
        #region - Test Classes -

        public abstract class SitesApiClientTest : ApiClientTestBase<ISitesApiClient>
        {
            internal SitesApiClient SitesApiClient => GetApiClient<SitesApiClient>();
        }

        #endregion

        #region - Ctor -

        public class Ctor : SitesApiClientTest
        {
            [Fact]
            public void Initializes_clients()
            {
                Assert.NotNull(SitesApiClient.Groups);
                Assert.NotNull(SitesApiClient.Jobs);
                Assert.NotNull(SitesApiClient.Projects);
                Assert.NotNull(SitesApiClient.Users);
            }
        }

        #endregion

        #region - GetSiteAsync (Id) -

        public class GetSiteAsyncById : SitesApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<SiteResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var siteId = Guid.NewGuid();

                var result = await SitesApiClient.GetSiteAsync(siteId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{siteId}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<SiteResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var siteId = Guid.NewGuid();

                var result = await SitesApiClient.GetSiteAsync(siteId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{siteId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var siteResponse = AutoFixture.CreateResponse<SiteResponse>();

                var mockResponse = new MockHttpResponseMessage<SiteResponse>(siteResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var siteId = Guid.NewGuid();

                var result = await SitesApiClient.GetSiteAsync(siteId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{siteId}");
            }
        }

        #endregion

        #region - GetSiteAsync (Content URL) -

        public class GetSiteAsyncByContentUrl : SitesApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<SiteResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var contentUrl = Create<string>();

                var result = await SitesApiClient.GetSiteAsync(contentUrl, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{contentUrl}");
                request.AssertQuery("key", "contentUrl");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<SiteResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var contentUrl = Create<string>();

                var result = await SitesApiClient.GetSiteAsync(contentUrl, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{contentUrl}");
                request.AssertQuery("key", "contentUrl");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var siteResponse = AutoFixture.CreateResponse<SiteResponse>();

                var mockResponse = new MockHttpResponseMessage<SiteResponse>(siteResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var contentUrl = Create<string>();

                var result = await SitesApiClient.GetSiteAsync(contentUrl, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{contentUrl}");
                request.AssertQuery("key", "contentUrl");
            }
        }

        #endregion

        #region - SignOutAsync -

        public class SignOutAsync : SitesApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                MockSessionProvider.SetupGet(p => p.UserId).Returns(Guid.NewGuid());

                var mockResponse = new MockHttpResponseMessage();

                MockHttpClient.SetupResponse(mockResponse);

                var result = await SitesApiClient.SignOutAsync(Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/auth/signout");

                MockSessionProvider.Verify(p => p.ClearCurrentUserAndSite(), Times.Once);
            }

            [Fact]
            public async Task Returns_failure()
            {
                MockSessionProvider.SetupGet(p => p.UserId).Returns(Guid.NewGuid());

                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await SitesApiClient.SignOutAsync(Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/auth/signout");

                MockSessionProvider.Verify(p => p.ClearCurrentUserAndSite(), Times.Once);
            }

            [Fact]
            public async Task Skips_when_no_current_user()
            {
                MockSessionProvider.SetupGet(p => p.UserId).Returns((Guid?)null);

                var result = await SitesApiClient.SignOutAsync(Cancel);

                Assert.True(result.Success);

                MockHttpClient.AssertNoRequests();
            }
        }

        #endregion

        #region - DisposeAsync -

        public class DisposeAsync : SitesApiClientTest
        {
            [Fact]
            public async Task Signs_out()
            {
                MockSessionProvider.SetupGet(p => p.UserId).Returns(Guid.NewGuid());

                var mockResponse = new MockHttpResponseMessage();

                MockHttpClient.SetupResponse(mockResponse);

                var result = await SitesApiClient.SignOutAsync(Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/auth/signout");
            }

            [Fact]
            public async Task Catches_errors()
            {
                MockSessionProvider.SetupGet(p => p.UserId).Returns(Guid.NewGuid());

                MockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())).Throws(new Exception());

                // Does not throw
                await SitesApiClient.DisposeAsync();

                MockHttpClient.Verify(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        #endregion
    }
}
