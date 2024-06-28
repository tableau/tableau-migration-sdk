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

namespace Tableau.Migration.Content.Schedules
{
    /// <summary>
    /// Class that can efficiently cache TContent objects for a given TKey, and release resources whenever they are read.
    /// </summary>
    /// <remarks>Implementations should be thread safe due to parallel migration processing.</remarks>
    internal class VolatileCache<TKey, TContent>
        where TKey : struct
        where TContent : class
    {
        private Dictionary<TKey, TContent> _cache = new();

        private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

        private readonly Func<CancellationToken, Task<Dictionary<TKey, TContent>>> _loadCache;

        private bool _loaded = false;

        public VolatileCache(Func<CancellationToken, Task<Dictionary<TKey, TContent>>> loadCache)
        {
            _loadCache = loadCache;
        }

        /// <summary>
        /// Single-threaded read all values linked to a given key reference, and release it when the key is found.
        /// </summary>
        /// <param name="keyValue">The content key.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The cached value, or null.</returns>
        public async Task<TContent?> GetAndRelease(
            TKey keyValue,
            CancellationToken cancel)
        {
            TContent? cachedResult = null;

            await _writeSemaphore.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                if (!_loaded)
                {
                    _cache = await _loadCache(cancel).ConfigureAwait(false);

                    _loaded = true;
                }

                if (_cache.TryGetValue(keyValue, out cachedResult))
                {
                    _cache.Remove(keyValue);
                }
            }
            finally
            {
                _writeSemaphore.Release();
            }

            return cachedResult;
        }
    }
}
