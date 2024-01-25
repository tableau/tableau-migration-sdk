using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public class BreadthFirstPathHierarchyPagerTests
    {
        public class NextPageAsync : AutoFixtureTestBase
        {
            private readonly CancellationToken _cancel = new();

            private TestContentType CreateTestContent(string name, TestContentType? parent)
            {
                var result = Create<TestContentType>();

                if (parent is null)
                {
                    result.Location = new ContentLocation(name);
                }
                else
                {
                    result.Location = parent.Location.Append(name);
                }

                return result;
            }

            [Fact]
            public async Task InnerPagerErrorAsync()
            {
                var innerResult = PagedResult<TestContentType>.Failed(new Exception());

                var mockInnerPager = new Mock<IPager<TestContentType>>()
                { CallBase = true };
                mockInnerPager.Setup(x => x.NextPageAsync(_cancel))
                    .ReturnsAsync(innerResult);

                var pager = new BreadthFirstPathHierarchyPager<TestContentType>(mockInnerPager.Object, 2);

                var result = await pager.NextPageAsync(_cancel);

                result.AssertFailure();

                Assert.Equal(innerResult.Errors, result.Errors);
            }

            [Fact]
            public async Task EmptyInnerPagerAsync()
            {
                var innerPager = new MemoryPager<TestContentType>(Array.Empty<TestContentType>(), 2);
                var pager = new BreadthFirstPathHierarchyPager<TestContentType>(innerPager, 2);

                var result = await pager.NextPageAsync(_cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Equal(1, result.PageNumber);
                Assert.Equal(2, result.PageSize);
                Assert.Equal(0, result.TotalCount);
                Assert.Empty(result.Value);
            }

            [Fact]
            public async Task PagesByOrderedHierarchyAsync()
            {
                var top1 = CreateTestContent("a", null);
                var child1 = CreateTestContent("a", top1);
                var child2 = CreateTestContent("c", top1);

                var top2 = CreateTestContent("b", null);
                var child3 = CreateTestContent("b", top2);
                var grandchild = CreateTestContent("a", child2);

                var top3 = CreateTestContent("c", null);

                //Unordered hierarchy.
                var hierarchyList = new[]
                {
                    top1, child1, child3, child2, grandchild,
                    top3,
                    top2,
                };

                var innerPager = new MemoryPager<TestContentType>(hierarchyList, 2);

                var pager = new BreadthFirstPathHierarchyPager<TestContentType>(innerPager, 2);

                //First page should have top items.
                var pageResult = await pager.NextPageAsync(_cancel);

                pageResult.AssertSuccess();
                Assert.NotNull(pageResult.Value);
                Assert.Equal(1, pageResult.PageNumber);
                Assert.Equal(2, pageResult.PageSize);
                Assert.Equal(hierarchyList.Length, pageResult.TotalCount);
                Assert.Equal(2, pageResult.Value.Count);
                Assert.Contains(top1, pageResult.Value);
                Assert.Contains(top2, pageResult.Value);

                //Second page should be the last top item, but not go further to children.
                pageResult = await pager.NextPageAsync(_cancel);

                pageResult.AssertSuccess();
                Assert.NotNull(pageResult.Value);
                Assert.Equal(2, pageResult.PageNumber);
                Assert.Equal(2, pageResult.PageSize);
                Assert.Equal(hierarchyList.Length, pageResult.TotalCount);
                Assert.Single(pageResult.Value);
                Assert.Contains(top3, pageResult.Value);

                //Third page should be filled with ordered child items.
                pageResult = await pager.NextPageAsync(_cancel);

                pageResult.AssertSuccess();
                Assert.NotNull(pageResult.Value);
                Assert.Equal(3, pageResult.PageNumber);
                Assert.Equal(2, pageResult.PageSize);
                Assert.Equal(hierarchyList.Length, pageResult.TotalCount);
                Assert.Equal(2, pageResult.Value.Count);
                Assert.Contains(child1, pageResult.Value);
                Assert.Contains(child2, pageResult.Value);

                //Fourth page should be the last child item but not go further to grandchildren.
                pageResult = await pager.NextPageAsync(_cancel);

                pageResult.AssertSuccess();
                Assert.NotNull(pageResult.Value);
                Assert.Equal(4, pageResult.PageNumber);
                Assert.Equal(2, pageResult.PageSize);
                Assert.Equal(hierarchyList.Length, pageResult.TotalCount);
                Assert.Single(pageResult.Value);
                Assert.Contains(child3, pageResult.Value);

                //Fifth page should be the grandchildren.
                pageResult = await pager.NextPageAsync(_cancel);

                pageResult.AssertSuccess();
                Assert.NotNull(pageResult.Value);
                Assert.Equal(5, pageResult.PageNumber);
                Assert.Equal(2, pageResult.PageSize);
                Assert.Equal(hierarchyList.Length, pageResult.TotalCount);
                Assert.Single(pageResult.Value);
                Assert.Contains(grandchild, pageResult.Value);

                //Sixth page should be the empty - we went past our data.
                pageResult = await pager.NextPageAsync(_cancel);

                pageResult.AssertSuccess();
                Assert.NotNull(pageResult.Value);
                Assert.Equal(6, pageResult.PageNumber);
                Assert.Equal(2, pageResult.PageSize);
                Assert.Equal(hierarchyList.Length, pageResult.TotalCount);
                Assert.Empty(pageResult.Value);
            }
        }
    }
}
