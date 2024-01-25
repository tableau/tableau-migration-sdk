using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class RestProjectResponsePagerTests
    {
        public class GetPageAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task UsesApiClientAsync()
            {
                var cancel = new CancellationToken();
                var expectedResult = Freeze<IPagedResult<ProjectsResponse.ProjectType>>();
                var mockApiClient = Create<Mock<IProjectsResponseApiClient>>();

                var pager = new RestProjectResponsePager(mockApiClient.Object, 123);

                var result = await pager.NextPageAsync(cancel);

                Assert.Same(expectedResult, result);
                mockApiClient.Verify(x => x.GetAllProjectsAsync(1, 123, cancel), Times.Once());
            }
        }
    }
}
