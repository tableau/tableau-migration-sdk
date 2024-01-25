using System;
using System.Net;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public abstract class ApiPageAccessorTestBase<TApiClient, TContent, TResponse> : ApiClientTestBase<TApiClient>
        where TApiClient : IApiPageAccessor<TContent>
        where TResponse : TableauServerResponse, new()
    {
        [Fact]
        public async Task GetPage_success()
        {
            // Arrange
            var response = AutoFixture.CreateResponse<TResponse>();

            var mockResponse = new MockHttpResponseMessage<TResponse>(response);

            MockHttpClient.SetupResponse(mockResponse);

            // Act
            var result = await ApiClient.GetPageAsync(2, 15, Cancel);

            // Assert
            result.AssertSuccess();

            var request = MockHttpClient.AssertSingleRequest();

            request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            request.AssertQuery("pageSize", "15");
            request.AssertQuery("pageNumber", "2");
        }

        [Fact]
        public async Task GetPage_failure()
        {
            // Arrange
            var exception = new Exception();

            var mockResponse = new MockHttpResponseMessage<TResponse>(HttpStatusCode.InternalServerError, null);
            mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

            MockHttpClient.SetupResponse(mockResponse);

            // Act
            var result = await ApiClient.GetPageAsync(3, 47, Cancel);

            // Assert
            result.AssertFailure();

            var request = MockHttpClient.AssertSingleRequest();

            request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            request.AssertQuery("pageSize", "47");
            request.AssertQuery("pageNumber", "3");
        }
    }
}
