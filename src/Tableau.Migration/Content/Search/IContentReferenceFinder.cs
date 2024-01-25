using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Interface for an object that can find <see cref="IContentReference"/>s
    /// for a given content type and search criteria.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IContentReferenceFinder<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Finds the content reference by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The found content reference, or null if no content reference was found.</returns>
        Task<IContentReference?> FindByIdAsync(Guid id, CancellationToken cancel);
    }
}
