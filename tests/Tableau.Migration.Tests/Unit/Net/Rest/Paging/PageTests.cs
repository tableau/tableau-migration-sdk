using Tableau.Migration.Net.Rest.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Paging
{
    public class PageTests
    {
        public abstract class PageTest : AutoFixtureTestBase
        { }

        public class Ctor : PageTest
        {
            [Fact]
            public void Sets_PageSize()
            {
                var size = Create<int>();

                var page = new Page(Create<int>(), size);

                Assert.Equal(size, page.PageSize);
            }

            [Fact]
            public void Sets_PageNumber()
            {
                var number = Create<int>();

                var page = new Page(number, Create<int>());

                Assert.Equal(number, page.PageNumber);
            }
        }
    }
}
