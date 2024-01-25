using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public abstract class PagedListApiClientTestBase<TApiClient, TContent, TResponse> : ApiClientTestBase<TApiClient>
        where TApiClient : IPagedListApiClient<TContent>
        where TResponse : TableauServerResponse, new()
    {
        [Fact]
        public async Task GetPager_GetsPage()
        {
            // Arrange
            var response = AutoFixture.CreateResponse<TResponse>();

            var mockResponse = new MockHttpResponseMessage<TResponse>(response);

            MockHttpClient.SetupResponse(mockResponse);

            // Act
            var pager = ApiClient.GetPager(123);
            var pageResult = await pager.NextPageAsync(Cancel);

            // Assert

            Assert.IsType<ApiListPager<TContent>>(pager);
            pageResult.AssertSuccess();

            var request = MockHttpClient.AssertSingleRequest();

            request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            request.AssertQuery("pageSize", "123");
            request.AssertQuery("pageNumber", "1");
        }
    }
}
