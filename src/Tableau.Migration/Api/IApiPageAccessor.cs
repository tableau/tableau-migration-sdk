using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an object that can list a single page of the content items the user has access to.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IApiPageAccessor<TContent> : IContentApiClient
    {
        /// <summary>
        /// Gets a single page of the content items that the user has access to.
        /// </summary>
        /// <param name="pageNumber">The 1-indexed page number for the page to list.</param>
        /// <param name="pageSize">The expected maximum number of items to include in the page.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The paged results.</returns>
        Task<IPagedResult<TContent>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel);
    }
}
