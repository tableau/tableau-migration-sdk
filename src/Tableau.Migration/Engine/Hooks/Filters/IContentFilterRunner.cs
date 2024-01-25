using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Interface for an object that can run filters.
    /// </summary>
    public interface IContentFilterRunner
    {
        /// <summary>
        /// Executes all filters for the content type in order.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="itemsToFilter">The items to filter.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>A lazy-evaluated collection of filterered items.</returns>
        Task<IEnumerable<ContentMigrationItem<TContent>>> ExecuteAsync<TContent>(IEnumerable<ContentMigrationItem<TContent>> itemsToFilter, CancellationToken cancel)
            where TContent : IContentReference;
    }
}
