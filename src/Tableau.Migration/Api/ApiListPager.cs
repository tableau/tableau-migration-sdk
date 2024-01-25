using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    internal sealed class ApiListPager<TContent> : IndexedPagerBase<TContent>
    {
        private readonly IApiPageAccessor<TContent> _listClient;

        public ApiListPager(IApiPageAccessor<TContent> listClient, int pageSize)
            : base(pageSize)
        {
            _listClient = listClient;
        }

        /// <inheritdoc />
        protected override async Task<IPagedResult<TContent>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await _listClient.GetPageAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);
    }
}
