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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Paging
{
    /// <summary>
    /// <see cref="IPager{TContent}"/> implementation that wraps an in-memory collection.
    /// </summary>
    public class MemoryPager<TItem> : MemoryPagerBase<TItem>
    {
        private readonly Func<CancellationToken, Task<IResult<IReadOnlyCollection<TItem>>>> _getItems;

        /// <summary>
        /// Creates a new <see cref="MemoryPager{TItem}"/> object.
        /// </summary>
        /// <param name="getItems">Function to get items asynchronously.</param>
        /// <param name="pageSize">The page size to page by.</param>
        public MemoryPager(Func<CancellationToken, Task<IResult<IReadOnlyCollection<TItem>>>> getItems, int pageSize)
            : base(pageSize)
        {
            _getItems = getItems;
        }

        /// <summary>
        /// Creates a new <see cref="MemoryPager{TItem}"/> object.
        /// </summary>
        /// <param name="items">The items to page through.</param>
        /// <param name="pageSize">The page size to page by.</param>
        public MemoryPager(IReadOnlyCollection<TItem> items, int pageSize)
            : this((c) => Task.FromResult((IResult<IReadOnlyCollection<TItem>>)Result<IReadOnlyCollection<TItem>>.Succeeded(items)), pageSize)
        { }

        /// <summary>
        /// Loads items asynchronously.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The loaded items result.</returns>
        protected override async Task<IResult<IReadOnlyCollection<TItem>>> LoadItemsAsync(CancellationToken cancel)
            => await _getItems(cancel).ConfigureAwait(false);
    }
}
