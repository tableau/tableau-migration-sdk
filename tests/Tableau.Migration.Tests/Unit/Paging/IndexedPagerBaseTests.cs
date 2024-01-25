using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public class IndexedPagerBaseTests
    {
        public class TestPager : IndexedPagerBase<TestContentType>
        {
            public List<int> CalledPageNumbers { get; } = new();

            public TestPager(int pageSize)
                : base(pageSize)
            { }

            protected override Task<IPagedResult<TestContentType>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            {
                CalledPageNumbers.Add(pageNumber);

                var result = PagedResult<TestContentType>.Succeeded(ImmutableArray<TestContentType>.Empty, pageNumber, pageSize, 2 * pageSize);
                return Task.FromResult((IPagedResult<TestContentType>)result);
            }
        }

        public class NextPageAsync
        {
            private readonly CancellationToken _cancel = new();

            [Fact]
            public async Task StartsAtPageOneAsync()
            {
                var pager = new TestPager(100);
                await pager.NextPageAsync(_cancel);

                Assert.Equal(new[] { 1 }, pager.CalledPageNumbers);
            }

            [Fact]
            public async Task IncrementsPageNumberAsync()
            {
                var pager = new TestPager(100);

                await pager.NextPageAsync(_cancel);
                await pager.NextPageAsync(_cancel);

                Assert.Equal(new[] { 1, 2 }, pager.CalledPageNumbers);
            }
        }
    }
}
