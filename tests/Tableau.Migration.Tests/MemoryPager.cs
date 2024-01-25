using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// <see cref="IPager{TContent}"/> implementation that wraps an in-memory collection for mocking purposes.
    /// </summary>
    internal class MemoryPager<TItem> : IPager<TItem>
    {
        private readonly IReadOnlyCollection<TItem> _items;
        private readonly int _pageSize;

        private int _pageNumber;
        private int _offset;

        public MemoryPager(IReadOnlyCollection<TItem> items, int pageSize)
        {
            _items = items;
            _pageSize = pageSize;

            _offset = 0;
            _pageNumber = 1;
        }

        public Task<IPagedResult<TItem>> NextPageAsync(CancellationToken cancel)
        {
            var pageItems = _items.Skip(_offset).Take(_pageSize).ToImmutableArray();
            var result = PagedResult<TItem>.Succeeded(pageItems, _pageNumber, _pageSize, _items.Count);

            _offset += _pageSize;
            _pageNumber++;

            return Task.FromResult<IPagedResult<TItem>>(result);
        }
    }
}
