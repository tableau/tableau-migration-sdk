using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Interface for an object that can efficiently cache <see cref="IContentReference"/> objects for a given endpoint and content type.
    /// </summary>
    /// <remarks>Implementations should be thread safe due to parallel migration processing.</remarks>
    public interface IContentReferenceCache
    {
        /// <summary>
        /// Finds the content reference item for a given endpoint location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The content reference, or null if no item was found.</returns>
        Task<IContentReference?> ForLocationAsync(ContentLocation location, CancellationToken cancel);

        /// <summary>
        /// Finds the content reference item for a given endpoint ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The content reference, or null if no item was found.</returns>
        Task<IContentReference?> ForIdAsync(Guid id, CancellationToken cancel);
    }
}
