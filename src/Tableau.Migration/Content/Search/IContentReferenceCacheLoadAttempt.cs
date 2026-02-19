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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Thread-safe interface representing an attempt to load a <see cref="IContentReferenceCache"/>
    /// during a cache miss.
    /// </summary>
    public interface IContentReferenceCacheLoadAttempt<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Finds whether the currently searched item is currently found in the cache.
        /// </summary>
        /// <returns>True if the searched item is currently in the cache, otherwise false.</returns>
        bool IsItemLoaded();

        /// <summary>
        /// Performs an individual item load based on the current search key, when supported, 
        /// and opportunistically loading items when available. 
        /// </summary>
        /// <param name="cancel">The cancellation token to obey</param>
        /// <returns>The result of the individual item load, which possibly might have been skipped due to the search mode not being supported.</returns>
        ValueTask<ContentReferenceLoadResult<TContent>> LoadItemAsync(CancellationToken cancel);

        /// <summary>
        /// Performs an eager load according to the re-load rules of the cache, opportunistically loading all items.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey</param>
        /// <returns>The task to await.</returns>
        ValueTask LoadAllAsync(CancellationToken cancel);
    }
}
