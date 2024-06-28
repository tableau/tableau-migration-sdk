//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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
        private bool _loaded = false;

        /// <summary>
        /// Gets the count of items in the cache.
        /// </summary>
        public int Count => _locationCache.Count;

        /// <summary>
        /// Searches for content at the given location, possibly returning more locations to opportunistically cache.
        /// </summary>
        /// <param name="searchLocation">The primary location to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content references to cache.</returns>
        protected abstract ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(ContentLocation searchLocation, CancellationToken cancel);

        /// <summary>
        /// Searches for content at the given ID, possibly returning more references to opportunistically cache.
        /// </summary>
        /// <param name="searchId">The primary ID to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content references to cache.</returns>
        protected abstract ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(Guid searchId, CancellationToken cancel);

        /// <summary>
        /// Searches for content at the given location.
        /// </summary>
        /// <param name="searchLocation">The primary location to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content reference to cache, or null.</returns>
        protected virtual Task<ContentReferenceStub?> IndividualSearchAsync(ContentLocation searchLocation, CancellationToken cancel)
            => Task.FromResult<ContentReferenceStub?>(null);

        /// <summary>
        /// Searches for content at the given ID.
        /// </summary>
        /// <param name="searchId">The primary ID to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content reference to cache, or null.</returns>
        protected virtual Task<ContentReferenceStub?> IndividualSearchAsync(Guid searchId, CancellationToken cancel)
            => Task.FromResult<ContentReferenceStub?>(null);

        private async Task<IContentReference?> SearchCacheAsync<TKey>(
            Dictionary<TKey, ContentReferenceStub?> cache, TKey search,
            Func<TKey, CancellationToken, ValueTask<IEnumerable<ContentReferenceStub>>> searchAsync,
            Func<TKey, CancellationToken, Task<ContentReferenceStub?>> individualSearchAsync,
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
                // Retry lookup in case a semaphore wait means the populated for this attempt.
                if (cache.TryGetValue(search, out cachedResult))
                {
                    return cachedResult;
                }

                if (!_loaded)
                {
                    // Load the cache with list values just once
                    var searchResults = await searchAsync(search, cancel).ConfigureAwait(false);
                    foreach (var searchResult in searchResults)
                    {
                        _idCache[searchResult.Id] = searchResult;
                        _locationCache[searchResult.Location] = searchResult;
                    }

                    _loaded = true;

                    // Retry lookup now that this attempt populated.
                    if (cache.TryGetValue(search, out cachedResult))
                    {
                        return cachedResult;
                    }
                }
                // No cached results. Retry individual search.
                cachedResult = await individualSearchAsync(search, cancel).ConfigureAwait(false);
                
                // Checks the individual search result.
                if (cachedResult is null)
                {
                    // Assign an explicit null if this fails to avoid repeated populations that will fail.
                    cache[search] = null;
                }
                else
                {
                    // Sets the cache with the individual search result.
                    _idCache[cachedResult.Id] = cachedResult;
                    _locationCache[cachedResult.Location] = cachedResult;
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
            => await SearchCacheAsync(_locationCache, location, SearchAsync, IndividualSearchAsync, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IContentReference?> ForIdAsync(Guid id, CancellationToken cancel)
            => await SearchCacheAsync(_idCache, id, SearchAsync, IndividualSearchAsync, cancel).ConfigureAwait(false);

        #endregion
    }
}
