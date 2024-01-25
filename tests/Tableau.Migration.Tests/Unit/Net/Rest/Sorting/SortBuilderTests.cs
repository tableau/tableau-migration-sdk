using System.Linq;
using Moq;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest.Sorting;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Sorting
{
    internal class SortBuilderTests
    {
        public abstract class SortBuilderTest : AutoFixtureTestBase
        {
            internal readonly SortBuilder Builder = new();
        }

        public class Build : SortBuilderTest
        {
            [Fact]
            public void Builds_single_sort()
            {
                var sort = Create<Sort>();

                Builder.AddSort(sort);

                Assert.Equal($"sort={sort.Expression}", Builder.Build());
            }

            [Fact]
            public void Builds_multiple_sorts()
            {
                var sorts = CreateMany<Sort>(2).ToList();

                Builder.AddSorts(sorts.ToArray());

                Assert.Equal($"sort={sorts[0].Expression},{sorts[1].Expression}", Builder.Build());
            }
        }

        public class AppendQueryString : SortBuilderTest
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
                var sort = Create<Sort>();

                Builder.AddSort(sort);

                Assert.False(Builder.IsEmpty);

                Builder.AppendQueryString(MockQuery.Object);

                MockQuery.Verify(q => q.AddOrUpdate("sort", sort.Expression), Times.Once);
            }
        }
    }
}
