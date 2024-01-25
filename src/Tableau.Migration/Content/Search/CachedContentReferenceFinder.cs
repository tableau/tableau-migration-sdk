using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// <see cref="IContentReferenceFinder{TContent}"/> implementation that uses 
    /// a <see cref="IContentReferenceCache"/> to find content references.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class CachedContentReferenceFinder<TContent> : IContentReferenceFinder<TContent>
        where TContent : IContentReference
    {
        private readonly IContentReferenceCache _cache;

        /// <summary>
        /// Creates a new <see cref="CachedContentReferenceFinder{TContent}"/> object.
        /// </summary>
        /// <param name="cache">The content reference cache.</param>
        public CachedContentReferenceFinder(IContentReferenceCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindByIdAsync(Guid id, CancellationToken cancel)
            => await _cache.ForIdAsync(id, cancel).ConfigureAwait(false);
    }
}
