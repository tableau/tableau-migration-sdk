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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// <see cref="IPager{TContent}"/> implementation that wraps an in-memory collection for mocking purposes.
    /// </summary>
    internal class MemoryPager<TItem> : IPager<TItem>
    {
        private readonly IReadOnlyCollection<TItem> _items;
        private readonly int _pageSize;

        private int _pageNumber;
        private int _offset;

        public MemoryPager(IReadOnlyCollection<TItem> items, int pageSize)
        {
            _items = items;
            _pageSize = pageSize;

            _offset = 0;
            _pageNumber = 1;
        }

        public Task<IPagedResult<TItem>> NextPageAsync(CancellationToken cancel)
        {
            var pageItems = _items.Skip(_offset).Take(_pageSize).ToImmutableArray();
            var result = PagedResult<TItem>.Succeeded(pageItems, _pageNumber, _pageSize, _items.Count, !pageItems.Any());

            _offset += _pageSize;
            _pageNumber++;

            return Task.FromResult<IPagedResult<TItem>>(result);
        }
    }
}
