using Moq;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Paging
{
    public class PageBuilderTests
    {
        public abstract class PageBuilderTest : AutoFixtureTestBase
        {
            internal readonly PageBuilder Builder = new();
        }

        public class IsEmpty : PageBuilderTest
        {
            [Fact]
            public void True()
            {
                Assert.True(Builder.IsEmpty);
            }

            [Fact]
            public void False()
            {
                var page = Create<Page>();

                Builder.SetPage(page);

                Assert.False(Builder.IsEmpty);
            }
        }

        public class Build : PageBuilderTest
        {
            [Fact]
            public void Builds_page()
            {
                var page = Create<Page>();

                Builder.SetPage(page);

                Assert.Equal($"pageSize={page.PageSize}&pageNumber={page.PageNumber}", Builder.Build());
            }
        }

        public class AppendQueryString : PageBuilderTest
        {
            protected readonly Mock<IQueryStringBuilder> MockQuery = new();

            [Fact]
            public void Skips_when_empty()
            {
                Assert.True(Builder.IsEmpty);

                Builder.AppendQueryString(MockQuery.Object);

                MockQuery.VerifyNoOtherCalls();
            }

            [Fact]
            public void Appends()
            {
                var page = Create<Page>();

                Builder.SetPage(page);

                Assert.False(Builder.IsEmpty);

                Builder.AppendQueryString(MockQuery.Object);

                MockQuery.Verify(q => q.AddOrUpdate("pageSize", page.PageSize.ToString()), Times.Once);
                MockQuery.Verify(q => q.AddOrUpdate("pageNumber", page.PageNumber.ToString()), Times.Once);
            }
        }
    }
}
