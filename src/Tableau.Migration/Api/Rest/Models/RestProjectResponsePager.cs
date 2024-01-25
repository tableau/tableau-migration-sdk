using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Pager implementation that pages raw project response objects, 
    /// which is used by <see cref="RestProjectBuilder"/> to build project locations
    /// with all parent projects available.
    /// </summary>
    internal sealed class RestProjectResponsePager : IndexedPagerBase<ProjectsResponse.ProjectType>
    {
        private readonly IProjectsResponseApiClient _apiClient;

        public RestProjectResponsePager(IProjectsResponseApiClient apiClient, int pageSize)
            : base(pageSize)
        {
            _apiClient = apiClient;
        }

        protected override async Task<IPagedResult<ProjectsResponse.ProjectType>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await _apiClient.GetAllProjectsAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);
    }
}
