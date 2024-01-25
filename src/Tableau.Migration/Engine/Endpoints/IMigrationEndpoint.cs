using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that represents a location to move Tableau data to or from.
    /// </summary>
    public interface IMigrationEndpoint : IAsyncDisposable
    {
        /// <summary>
        /// Performs pre-migration initialization.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>An awaitable task with the initialization result.</returns>
        Task<IResult> InitializeAsync(CancellationToken cancel);

        /// <summary>
        /// Gets a pager to list all the content the user has access to.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="pageSize">The page size to use.</param>
        /// <returns>A pager to list content with.</returns>
        IPager<TContent> GetPager<TContent>(int pageSize);
    }
}
