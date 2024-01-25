using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Pager implementation that pages group user members response objects
    /// for a given group.
    /// </summary>
    internal sealed class GroupUsersResponsePager
        : IndexedPagerBase<IUser>, IPager<IUser>
    {
        private readonly IGroupsApiClient _apiClient;
        private readonly Guid _groupId;

        public GroupUsersResponsePager(
            IGroupsApiClient apiClient,
            Guid groupId,
            int pageSize)
            : base(pageSize)
        {
            _apiClient = apiClient;
            _groupId = groupId;
        }

        protected override async Task<IPagedResult<IUser>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await _apiClient.GetGroupUsersAsync(_groupId, pageNumber, pageSize, cancel).ConfigureAwait(false);
    }
}
