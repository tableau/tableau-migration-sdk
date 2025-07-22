//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tableau.Migration.Config;

namespace Tableau.Migration.Engine.Caching
{
    /// <summary>
    /// <see cref="IMigrationCache{TKey, TValue}"/> implementation that allows for configurable memory caching.
    /// </summary>
    /// <typeparam name="TKey"><inheritdoc /></typeparam>
    /// <typeparam name="TValue"><inheritdoc /></typeparam>
    public abstract class ConfigurableMigrationCacheBase<TKey, TValue> : IMigrationCache<TKey, TValue>
        where TKey : notnull
        where TValue : class
    {
        #region - Properties -

        /// <summary>
        /// Gets the default memory cache options to merge with user options for this cache.
        /// </summary>
        protected virtual MemoryCacheOptions DefaultMemoryCacheOptions { get; } = new();

        /// <summary>
        /// Gets a semaphore to lock for thread safety when writing.
        /// </summary>
        protected SemaphoreSlim WriteSemaphore { get; } = new(1, 1);

        /// <summary>
        /// Gets the inner memory cache.
        /// </summary>
        protected MemoryCache MemoryCache { get; }

        #endregion

        #region - Constructors -

        /// <summary>
        /// Creates a new <see cref="ConfigurableMigrationCacheBase{TKey, TValue}"/> object.
        /// </summary>
        /// <param name="config">The configuration reader.</param>
        /// <param name="cacheKey">The unique key for the cache to use for configuration.</param>
        public ConfigurableMigrationCacheBase(IConfigReader config, string cacheKey)
        {
            var cacheOptions = BuildCacheOptions(config, cacheKey);

            MemoryCache = new MemoryCache(cacheOptions);
        }

        #endregion

        #region - Protected Methods -

        /// <summary>
        /// Builds a <see cref="MemoryCacheOptions"/> object from user configuration options and fallback default values specific to this cache.
        /// </summary>
        /// <param name="options">The user supplied configuration options.</param>
        /// <param name="defaultOptions">The fallback default values specific to this cache.</param>
        /// <returns>The merged <see cref="MemoryCacheOptions"/>.</returns>
        protected MemoryCacheOptions MergeOptions(CacheOptions options, MemoryCacheOptions defaultOptions)
        {
            return new()
            {
                SizeLimit = options.SizeLimit ?? defaultOptions.SizeLimit,
                TrackStatistics = defaultOptions.TrackStatistics
            };
        }

        /// <summary>
        /// Builds the memory cache options to use based on current configuration.
        /// </summary>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="cacheKey">The unique key for the cache to use for configuration.</param>
        /// <returns>The <see cref="MemoryCacheOptions"/> to use.</returns>
        protected virtual MemoryCacheOptions BuildCacheOptions(IConfigReader configReader, string cacheKey)
        {
            var config = configReader.Get();
            CacheOptions opts = config.Caches.TryGetValue(cacheKey, out var cacheOptions) ? cacheOptions : new();

            return MergeOptions(opts, DefaultMemoryCacheOptions);
        }

        /// <summary>
        /// Adds a find result to the cache for later use.
        /// </summary>
        /// <param name="key">The cache item key.</param>
        /// <param name="findResult">The result of the find operation.</param>
        protected virtual void Add(TKey key, IResult<TValue> findResult)
            => MemoryCache.Set(key, findResult, new MemoryCacheEntryOptions
            {
                Size = 1
            });

        /// <summary>
        /// Finds the value to cache for a given cache key.
        /// </summary>
        /// <param name="key">The key to find the value for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the find operation with the value to cache.</returns>
        protected abstract Task<IResult<TValue>> FindCacheMissAsync(TKey key, CancellationToken cancel);

        #endregion

        #region - IDisposable Implementation -

        private bool _disposed;

        /// <summary>
        /// Disposes of the cache.
        /// </summary>
        /// <param name="disposing">Whether or not to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    MemoryCache.Dispose();
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region - IMigrationCache<TKey, TValue> Implementation -

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
            => Add(key, Result<TValue>.Succeeded(value));

        /// <inheritdoc />
        public async Task<IResult<TValue>> GetOrAddAsync(TKey key, CancellationToken cancel)
        {
            IResult<TValue>? result;
            if (!MemoryCache.TryGetValue(key, out result) || result is null)
            {
                await WriteSemaphore.WaitAsync(cancel).ConfigureAwait(false);
                try
                {
                    if(!MemoryCache.TryGetValue(key, out result) || result is null)
                    {
                        var findResult = await FindCacheMissAsync(key, cancel).ConfigureAwait(false);
                        Add(key, findResult);
                        
                        return findResult;
                    }
                }
                finally
                {
                    WriteSemaphore.Release();
                }
            }

            return result;
        }

        #endregion
    }
}
