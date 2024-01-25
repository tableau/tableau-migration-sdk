using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ApiClientTests
    {
        public abstract class ApiClientTest : ApiTestBase
        {
            protected readonly Mock<ISitesApiClient> MockSitesApiClient = new();

            internal readonly ApiClient ApiClient;

            public ApiClientTest()
            {
                ApiClient = new(
                    MockApiClientInput.Object,
                    RestRequestBuilderFactory,
                    MockTokenProvider.Object,
                    MockSessionProvider.Object,
                    MockLoggerFactory.Object,
                    MockSitesApiClient.Object,
                    MockSharedResourcesLocalizer.Object);
            }
        }

        public class GetServerInfoAsync : ApiClientTest
        {
            [Fact]
            public async Task Returns_server_info()
            {
                MockSessionProvider.SetupGet(p => p.Version).Returns((TableauServerVersion?)null);

                var serverInfoResponse = AutoFixture.CreateResponse<ServerInfoResponse>();

                var mockResponse = new MockHttpResponseMessage<ServerInfoResponse>(serverInfoResponse);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetServerInfoAsync(Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri("/api/2.4/serverinfo");

                MockSessionProvider.Verify(p => p.SetVersion(result.Value.TableauServerVersion), Times.Once);
            }

            [Fact]
            public async Task Uses_configured_Rest_Api_version()
            {
                var originalVersion = TableauServerVersion;

                var serverInfoResponse = AutoFixture.CreateResponse<ServerInfoResponse>();

                var mockResponse = new MockHttpResponseMessage<ServerInfoResponse>(serverInfoResponse);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetServerInfoAsync(Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{originalVersion.RestApiVersion}/serverinfo");

                MockSessionProvider.Verify(p => p.SetVersion(result.Value.TableauServerVersion), Times.Once);
            }

            [Fact]
            public async Task Returns_error()
            {
                var serverInfoResponse = AutoFixture.CreateErrorResponse<ServerInfoResponse>();

                var mockResponse = new MockHttpResponseMessage<ServerInfoResponse>(serverInfoResponse);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetServerInfoAsync(Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertUri(SiteConnectionConfiguration.ServerUrl, $"/api/{TableauServerVersion.RestApiVersion}/serverinfo");

                MockSessionProvider.Verify(p => p.SetCurrentUserAndSite(It.IsAny<ISignInResult>()), Times.Never);
            }
        }

        public class SignInAsync : ApiClientTest
        {
            [Fact]
            public async Task Returns_site_client()
            {
                var signInResponse = AutoFixture.CreateResponse<SignInResponse>();

                var mockResponse = new MockHttpResponseMessage<SignInResponse>(signInResponse);

                MockHttpClient.SetupResponse(mockResponse);

                await using var result = await ApiClient.SignInAsync(Cancel);

                Assert.True(result.Success);
                Assert.Same(MockSitesApiClient.Object, result.Value);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/auth/signin");

                await request.AssertContentAsync<SignInRequest>(
                    Serializer,
                    r =>
                    {
                        Assert.Equal(r.Credentials!.Site!.ContentUrl, SiteConnectionConfiguration.SiteContentUrl);
                        Assert.Equal(r.Credentials.PersonalAccessTokenName, SiteConnectionConfiguration.AccessTokenName);
                        Assert.Equal(r.Credentials.PersonalAccessTokenSecret, SiteConnectionConfiguration.AccessToken);
                    });

                MockSessionProvider.Verify(p => p.SetCurrentUserAndSite(It.Is<SignInResult>(r =>
                    r.SiteId == signInResponse.Item!.Site!.Id &&
                    r.SiteContentUrl == signInResponse.Item.Site.ContentUrl &&
                    r.UserId == signInResponse.Item.User!.Id &&
                    r.Token == signInResponse.Item.Token)),
                    Times.Once);
            }

            [Fact]
            public async Task Gets_Rest_Api_version_if_not_set()
            {
                MockSessionProvider.SetupGet(p => p.Version).Returns((TableauServerVersion?)null);

                var serverInfoVersion = Create<TableauServerVersion>();

                var serverInfoResponse = AutoFixture.CreateResponse<ServerInfoResponse>();
                serverInfoResponse.Item!.RestApiVersion!.Version = serverInfoVersion.RestApiVersion;

                var mockServerInfoResponse = new MockHttpResponseMessage<ServerInfoResponse>(serverInfoResponse);

                MockHttpClient.SetupResponse(mockServerInfoResponse);

                var mockSignInResponse = new MockHttpResponseMessage<SignInResponse>(AutoFixture.CreateResponse<SignInResponse>());

                MockHttpClient.SetupResponse(mockSignInResponse);

                await using var result = await ApiClient.SignInAsync(Cancel);

                var requests = MockHttpClient.AssertRequestCount(2);

                var signInRequest = requests[1];

                signInRequest.AssertRelativeUri($"/api/{serverInfoVersion.RestApiVersion}/auth/signin");

                Assert.True(result.Success);
            }

            [Fact]
            public async Task Returns_error()
            {
                var signInResponse = AutoFixture.CreateErrorResponse<SignInResponse>();

                var mockResponse = new MockHttpResponseMessage<SignInResponse>(signInResponse);

                MockHttpClient.SetupResponse(mockResponse);

                await using var result = await ApiClient.SignInAsync(Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/auth/signin");
            }
        }
    }
}
