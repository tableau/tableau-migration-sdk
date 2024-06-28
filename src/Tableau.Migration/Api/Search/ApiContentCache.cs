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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api.Search
{
    /// <summary>
    /// Thread-safe <see cref="IContentCache{TContent}"/> implementation.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ApiContentCache<TContent> : BulkApiContentReferenceCache<TContent>, IContentCache<TContent>
        where TContent : class, IContentReference
    {
        private readonly ConcurrentDictionary<ContentReferenceStub, TContent> _innerCache;

        /// <summary>
        /// Creates a new <see cref="ApiContentCache{TContent}"/> instance.
        /// </summary>
        /// <param name="apiClient">An API client.</param>
        /// <param name="configReader">A config reader.</param>
        public ApiContentCache(ISitesApiClient? apiClient, IConfigReader configReader)
            : this(apiClient, configReader, new())
        { }

        /// <summary>
        /// Creates a new <see cref="ApiContentCache{TContent}"/> instance.
        /// </summary>
        /// <param name="apiClient">An API client.</param>
        /// <param name="configReader">A config reader.</param>
        /// <param name="innerCache">The inner content dictionary for testing.</param>
        internal ApiContentCache(ISitesApiClient? apiClient, IConfigReader configReader, ConcurrentDictionary<ContentReferenceStub, TContent> innerCache)
            : base(apiClient, configReader)
        {
            _innerCache = innerCache;
        }

        /// <inheritdoc />
        new public async Task<TContent?> ForLocationAsync(ContentLocation location, CancellationToken cancel)
            => await ForReferenceAsync(() => base.ForLocationAsync(location, cancel)).ConfigureAwait(false);

        /// <inheritdoc />
        new public async Task<TContent?> ForIdAsync(Guid id, CancellationToken cancel)
            => await ForReferenceAsync(() => base.ForIdAsync(id, cancel)).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<TContent?> ForReferenceAsync(IContentReference reference, CancellationToken cancel)
            => await ForReferenceAsync(() => Task.FromResult<IContentReference?>(reference)).ConfigureAwait(false);

        /// <inheritdoc />
        public TContent AddOrUpdate(TContent item)
            => _innerCache.AddOrUpdate(item.ToStub(), (_) => item, (_, __) => item);

        /// <inheritdoc />
        public IImmutableList<TContent> AddOrUpdateRange(IEnumerable<TContent> items)
        {
            var results = ImmutableArray.CreateBuilder<TContent>();

            foreach (var item in items)
                results.Add(AddOrUpdate(item));

            return results.ToImmutable();
        }

        private async Task<TContent?> ForReferenceAsync(Func<Task<IContentReference?>> getReferenceAsync)
        {
            var reference = await getReferenceAsync().ConfigureAwait(false);

            if (reference is null)
                return null;

            return _innerCache.TryGetValue(reference.ToStub(), out var content) ? content : null;
        }

        /// <inheritdoc />
        protected override void ItemLoaded(TContent item)
        {
            AddOrUpdate(item);

            base.ItemLoaded(item);
        }
    }
}
