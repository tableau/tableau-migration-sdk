//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Abstract base class for thread-safe <see cref="IContentReferenceCache"/> implementations.
    /// </summary>
    public abstract class ContentReferenceCacheBase<TContent> : IContentReferenceCache
        where TContent : IContentReference
    {
        private readonly Dictionary<ContentLocation, ContentReferenceStub?> _locationCache = new();
        private readonly Dictionary<Guid, ContentReferenceStub?> _idCache = new();
        private readonly Dictionary<string, ContentReferenceStub?> _contentUrlCache = new();

        private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

        private readonly IContentReferenceCacheLoadStrategy<TContent> _loadStrategy;
        private readonly ILogger<ContentReferenceCacheBase<TContent>> _logger;

        /// <summary>
        /// Gets the content reference store.
        /// </summary>
        protected IContentReferenceStore<TContent> Store { get; }

        /// <summary>
        /// 
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        /// Creates a new <see cref="ContentReferenceCacheBase{TContent}"/> object.
        /// </summary>
        /// <param name="loadStrategy">The loading strategy.</param>
        /// <param name="store">The content reference store.</param>
        /// <param name="logger">The logger.</param>
        public ContentReferenceCacheBase(IContentReferenceCacheLoadStrategy<TContent> loadStrategy, 
            IContentReferenceStore<TContent> store, ILogger<ContentReferenceCacheBase<TContent>> logger)
        {
            _loadStrategy = loadStrategy;
            Store = store;
            _logger = logger;
        }

        /// <summary>
        /// Gets the count of items in the cache.
        /// </summary>
        public int Count => _locationCache.Count;

        /// <summary>
        /// Gets whether <see cref="SearchAllAsync"/> has been called before from any source.
        /// </summary>
        protected bool LoadedAll { get; private set; }

        /// <summary>
        /// Called after one or more items have been loaded into the cache from the store.
        /// </summary>
        /// <param name="items">The items that were loaded.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await.</returns>
        protected virtual Task ItemsLoadedAsync(IImmutableList<TContent> items, CancellationToken cancel) => Task.CompletedTask;

        private async Task ProcessLoadResultsAsync(IImmutableList<TContent> loadResults, CancellationToken cancel)
        {
            foreach(var loadResult in loadResults)
            {
                var stub = loadResult.ToStub();

                if (loadResult.Id != Guid.Empty)
                {
                    _idCache[loadResult.Id] = stub;
                }

                if (!string.IsNullOrEmpty(loadResult.ContentUrl))
                {
                    _contentUrlCache[loadResult.ContentUrl] = stub;
                }

                _locationCache[loadResult.Location] = stub;
            }            

            await ItemsLoadedAsync(loadResults, cancel).ConfigureAwait(false);

            _logger.LogDebug("{Name} content reference cache processed {Count} items.", Name, loadResults.Count);
        }

        private async ValueTask<IContentReference?> SearchCacheAsync<TKey>(Dictionary<TKey, ContentReferenceStub?> cache, TKey search,
            Func<TKey, CancellationToken, ValueTask<ContentReferenceLoadResult<TContent>>> loadByKeyAsync,
            CancellationToken cancel)
            where TKey : notnull
        {
            // A wrapper local callback to do individual item loading that ensures found items are properly populated in the cache.
            async ValueTask<ContentReferenceLoadResult<TContent>> SearchByKeyAsync(CancellationToken c)
            {
                _logger.LogDebug("{Name} content reference cache making individual search for key {Key}.", Name, search);
                var loadResult = await loadByKeyAsync(search, c).ConfigureAwait(false);
                await ProcessLoadResultsAsync(loadResult.Items, c).ConfigureAwait(false);

                return loadResult;
            }

            // First-chance cache test.
            if (cache.TryGetValue(search, out var cachedResult))
            {
                return cachedResult;
            }

            // Cache miss, obtain a write lock to populate cache.
            await _writeSemaphore.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                // Retry lookup in case a semaphore wait means the populated for this attempt.
                if (cache.TryGetValue(search, out cachedResult))
                {
                    return cachedResult;
                }

                _logger.LogInformation("{Name} content reference cache miss on search key {Key}.", Name, search);

                // Run the load strategy to call the store and perform fall-back operations.
                var loadAttempt = new ContentReferenceCacheLoadAttempt<TContent>(() => cache.ContainsKey(search), SearchAllAsync, SearchByKeyAsync);
                await _loadStrategy.LoadAsync(loadAttempt, cancel).ConfigureAwait(false);

                // Retry lookup now that this attempt populated.
                if (cache.TryGetValue(search, out cachedResult))
                {
                    return cachedResult;
                }
                else
                {
                    // Assign an explicit null if load failed, to avoid repeated loading that will likely also fail.
                    cache[search] = null;
                }

                return cachedResult;
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        /// <summary>
        /// Performs a one-time bulk search, loading all items into the cache if this is the first bulk search,
        /// and returning all cached items.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A <see cref="ValueTask"/> to await.</returns>
        protected async ValueTask SearchAllAsync(CancellationToken cancel)
        {
            if (!LoadedAll)
            {
                _logger.LogDebug("{Name} content reference cache making bulk search.", Name);

                var searchResults = await Store.LoadAllAsync(cancel).ConfigureAwait(false);
                await ProcessLoadResultsAsync(searchResults, cancel).ConfigureAwait(false);

                LoadedAll = true;
            }
        }

        #region - IContentReferenceCache Implementation -

        /// <inheritdoc />
        public async Task<IImmutableList<IContentReference>> GetAllAsync(CancellationToken cancel)
        {
            if (!LoadedAll)
            {
                await _writeSemaphore.WaitAsync(cancel).ConfigureAwait(false);

                try
                {
                    await SearchAllAsync(cancel).ConfigureAwait(false);
                }
                finally
                {
                    _writeSemaphore.Release();
                }
            }

            var results = _locationCache.Values
                .ExceptNulls()
                .ToImmutableArray<IContentReference>();

            return results;
        }

        /// <inheritdoc />
        public virtual async Task<IContentReference?> ForLocationAsync(ContentLocation location, CancellationToken cancel)
            => await SearchCacheAsync(_locationCache, location, Store.LoadAsync, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public virtual async Task<IContentReference?> ForIdAsync(Guid id, CancellationToken cancel)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            return await SearchCacheAsync(_idCache, id, Store.LoadAsync, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task<IContentReference?> ForContentUrlAsync(string contentUrl, CancellationToken cancel)
        {
            if (string.IsNullOrEmpty(contentUrl))
            {
                return null;
            }

            return await SearchCacheAsync(_contentUrlCache, contentUrl, Store.LoadAsync, cancel).ConfigureAwait(false);
        }

        #endregion
    }
}
