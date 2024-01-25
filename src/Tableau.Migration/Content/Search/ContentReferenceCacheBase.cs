using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Thread-safe <see cref="IContentReferenceCache"/> implementation.
    /// </summary>
    public abstract class ContentReferenceCacheBase : IContentReferenceCache
    {
        private readonly Dictionary<ContentLocation, ContentReferenceStub?> _locationCache = new();
        private readonly Dictionary<Guid, ContentReferenceStub?> _idCache = new();

        private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

        /// <summary>
        /// Gets the count of items in the cache.
        /// </summary>
        public int Count => _locationCache.Count;

        /// <summary>
        /// Searches for content at the given location, possibly returning more locations to opportunistically cache.
        /// </summary>
        /// <param name="searchLocation">The primary location to search for.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The content references to cache.</returns>
        protected abstract ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(ContentLocation searchLocation, CancellationToken cancel);

        /// <summary>
        /// Searches for content at the given ID, possibly returning more references to opportunistically cache.
        /// </summary>
        /// <param name="searchId">The primary ID to search for.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The content references to cache.</returns>
        protected abstract ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(Guid searchId, CancellationToken cancel);

        private async Task<IContentReference?> SearchCacheAsync<TKey>(
            Dictionary<TKey, ContentReferenceStub?> cache, TKey search,
            Func<TKey, CancellationToken, ValueTask<IEnumerable<ContentReferenceStub>>> searchAsync,
            CancellationToken cancel)
            where TKey : notnull
        {
            if (cache.TryGetValue(search, out var cachedResult))
            {
                return cachedResult;
            }

            await _writeSemaphore.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                //Retry lookup in case a semaphore wait means the populated for this attempt.
                if (cache.TryGetValue(search, out cachedResult))
                {
                    return cachedResult;
                }

                var searchResults = await searchAsync(search, cancel).ConfigureAwait(false);
                foreach (var searchResult in searchResults)
                {
                    _idCache[searchResult.Id] = searchResult;
                    _locationCache[searchResult.Location] = searchResult;
                }

                //Retry lookup now that this attempt populated.
                //Assign an explicit null if this fails to avoid repeated populations that will fail.
                if (!cache.TryGetValue(search, out cachedResult))
                {
                    cachedResult = cache[search] = null;
                }

                return cachedResult;
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        #region - IContentReferenceCache Implementation -

        /// <inheritdoc />
        public async Task<IContentReference?> ForLocationAsync(ContentLocation location, CancellationToken cancel)
            => await SearchCacheAsync(_locationCache, location, SearchAsync, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IContentReference?> ForIdAsync(Guid id, CancellationToken cancel)
            => await SearchCacheAsync(_idCache, id, SearchAsync, cancel).ConfigureAwait(false);

        #endregion
    }
}
