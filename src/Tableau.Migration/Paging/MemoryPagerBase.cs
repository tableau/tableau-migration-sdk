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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Paging
{
    /// <summary>
    /// <see cref="IPager{TContent}"/> implementation that wraps an in-memory collection.
    /// </summary>
    internal abstract class MemoryPagerBase<TItem> : IPager<TItem>
    {
        private readonly int _pageSize;

        private IReadOnlyCollection<TItem>? _items;
        private int _pagesAvailable;

        private int _pageNumber;
        private int _offset;

        public MemoryPagerBase(int pageSize)
        {
            _pageSize = pageSize;

            _offset = 0;
            _pageNumber = 1;
        }

        protected abstract Task<IResult<IReadOnlyCollection<TItem>>> LoadItemsAsync(CancellationToken cancel);

        public async Task<IPagedResult<TItem>> NextPageAsync(CancellationToken cancel)
        {
            if (_items is null)
            {
                var getResult = await LoadItemsAsync(cancel).ConfigureAwait(false);
                if (!getResult.Success)
                {
                    return PagedResult<TItem>.Failed(getResult.Errors);
                }

                _items = getResult.Value;
                _pagesAvailable = (_items.Count / _pageSize) + (_items.Count % _pageSize > 0 ? 1 : 0);
            }

            var pageItems = _items.Skip(_offset).Take(_pageSize).ToImmutableArray();
            var result = PagedResult<TItem>.Succeeded(pageItems, _pageNumber, _pageSize, _items.Count, _pageNumber >= _pagesAvailable);

            _offset += _pageSize;
            _pageNumber++;

            return result;
        }
    }
}
