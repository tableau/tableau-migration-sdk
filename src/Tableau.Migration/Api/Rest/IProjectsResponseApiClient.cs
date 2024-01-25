using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Rest
{
    internal interface IProjectsResponseApiClient
    {
        IPager<ProjectsResponse.ProjectType> GetPager(int pageSize);

        Task<IPagedResult<ProjectsResponse.ProjectType>> GetAllProjectsAsync(int pageNumber, int pageSize, CancellationToken cancel);

        Task<IPagedResult<ProjectsResponse.ProjectType>> GetAllProjectsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters, CancellationToken cancel);
    }
}
