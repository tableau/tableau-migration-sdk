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
    /// Content reference cache load strategy that prefers bulk loading,
    /// falling back to individual searches when necessary.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public sealed class BulkContentReferenceCacheLoadStrategy<TContent> : IContentReferenceCacheLoadStrategy<TContent>
        where TContent : IContentReference
    {
        /// <inheritdoc />
        public async Task LoadAsync(IContentReferenceCacheLoadAttempt<TContent> attempt, CancellationToken cancel)
        {
            // Bulk load first. The cache may elide loading to avoid repeated listing.
            await attempt.LoadAllAsync(cancel).ConfigureAwait(false);

            if(attempt.IsItemLoaded())
            {
                return;
            }

            // If bulk load found the item (e.g. items the default list can't see) we fall back to an immediate individual lookup.
            await attempt.LoadItemAsync(cancel).ConfigureAwait(false);
        }
    }
}
