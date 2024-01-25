using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class PagedTableauServerResponseTests
    {
        public class IPageInfoImplementation : AutoFixtureTestBase
        {
            [Fact]
            public void UsesPagerValues()
            {
                var response = Create<TestPagedTableauServerResponse>();

                Assert.Equal(response.Pagination.PageNumber, ((IPageInfo)response).PageNumber);
                Assert.Equal(response.Pagination.PageSize, ((IPageInfo)response).PageSize);
                Assert.Equal(response.Pagination.TotalAvailable, ((IPageInfo)response).TotalCount);
            }
        }
    }
}
