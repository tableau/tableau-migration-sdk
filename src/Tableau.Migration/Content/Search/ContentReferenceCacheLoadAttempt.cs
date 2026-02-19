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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    internal sealed record ContentReferenceCacheLoadAttempt<TContent> : IContentReferenceCacheLoadAttempt<TContent>
        where TContent : IContentReference
    {
        private readonly Func<bool> _isItemLoaded;
        private readonly Func<CancellationToken, ValueTask> _loadAllAsync;
        private readonly Func<CancellationToken, ValueTask<ContentReferenceLoadResult<TContent>>> _loadItemAsync;

        public ContentReferenceCacheLoadAttempt(Func<bool> isItemLoaded,
            Func<CancellationToken, ValueTask> loadAllAsync,
            Func<CancellationToken, ValueTask<ContentReferenceLoadResult<TContent>>> loadItemAsync)
        {
            _isItemLoaded = isItemLoaded;
            _loadAllAsync = loadAllAsync;
            _loadItemAsync = loadItemAsync;
        }

        public bool IsItemLoaded()
            => _isItemLoaded();

        /// <inheritdoc />
        public async ValueTask LoadAllAsync(CancellationToken cancel) 
            => await _loadAllAsync(cancel);

        /// <inheritdoc />
        public async ValueTask<ContentReferenceLoadResult<TContent>> LoadItemAsync(CancellationToken cancel)
            => await _loadItemAsync(cancel);
    }
}
