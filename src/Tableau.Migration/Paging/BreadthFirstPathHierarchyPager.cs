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
using Tableau.Migration.Content;

namespace Tableau.Migration.Paging
{
    /// <summary>
    /// <see cref="IPager{TContent}"/> implementation that
    /// produces pages from a breadth first traversal of a path hierarchy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This pager builds an in-memory hierarchy based on each item's location path, 
    /// and produces pages based on breadth first traversal of that hierarchy.
    /// All items are loaded from an inner pager, as this pager expects that 
    /// it is not possible to efficiently query pages based on the hierarchy level.
    /// Therefore this pager trades not doing exponential API/storage lookups
    /// at the cost of storing all items in memory.
    /// </para>
    /// <para>
    /// Pages never cross a boundary to the next hierarchy level, 
    /// even if this produces incomplete pages.
    /// This is done so that page items can be processed in a parallel without
    /// concern to whether an items parents have alredy been processed - 
    /// they are guaranteed to have been present in a previous page if they are present at all.
    /// </para>
    /// </remarks>
    internal class BreadthFirstPathHierarchyPager<TContent> : IPager<TContent>
        where TContent : IContentReference
    {
        private const int DEFAULT_PAGE = 1;

        private readonly IPager<TContent> _innerPager;
        private readonly int _pageSize;

        private bool _loaded;
        private ImmutableSortedDictionary<int, ImmutableSortedSet<TContent>> _itemsByHierarchyLevel;
        private int _totalCount;
        private int _currentPage = DEFAULT_PAGE;

        private int _currentLevel;
        private int _currentLevelPage = DEFAULT_PAGE;

        public BreadthFirstPathHierarchyPager(IPager<TContent> innerPager, int pageSize)
        {
            _innerPager = innerPager;
            _pageSize = pageSize;
            _itemsByHierarchyLevel = ImmutableSortedDictionary<int, ImmutableSortedSet<TContent>>.Empty;
        }

        private async Task<IResult> LoadHierarchyAsync(CancellationToken cancel)
        {
            var hierarchy = new Dictionary<int, ImmutableSortedSet<TContent>.Builder>();

            var getAllResult = await _innerPager.GetAllPagesAsync(cancel).ConfigureAwait(false);
            if (!getAllResult.Success)
            {
                return getAllResult;
            }

            foreach (var item in getAllResult.Value)
            {
                var level = item.Location.PathSegments.Length;
                if (!hierarchy.TryGetValue(level, out var levelSet))
                {
                    hierarchy[level] = levelSet = ImmutableSortedSet.CreateBuilder(ContentLocationComparer<TContent>.Instance);
                }

                levelSet.Add(item);
            }

            _itemsByHierarchyLevel = hierarchy.ToImmutableSortedDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutable());
            if (_itemsByHierarchyLevel.Any())
            {
                _currentLevel = _itemsByHierarchyLevel.Keys.First();
            }

            _totalCount = getAllResult.Value.Count;

            _loaded = true;
            return Result.Succeeded();
        }

        public async Task<IPagedResult<TContent>> NextPageAsync(CancellationToken cancel)
        {
            if (!_loaded)
            {
                var loadResult = await LoadHierarchyAsync(cancel).ConfigureAwait(false);
                if (!loadResult.Success)
                {
                    return PagedResult<TContent>.Failed(loadResult.Errors);
                }
            }

            //Handle empty set.
            if (!_itemsByHierarchyLevel.Any())
            {
                return PagedResult<TContent>.Succeeded(ImmutableArray<TContent>.Empty, _currentPage, _pageSize, _totalCount, true);
            }

            //See if there is any items left in our current level.
            var level = _itemsByHierarchyLevel[_currentLevel];
            var levelPage = level.Skip((_currentLevelPage - 1) * _pageSize).Take(_pageSize).ToImmutableArray();

            _currentLevelPage++;

            var peekNextPage = level.Skip((_currentLevelPage - 1) * _pageSize).Take(_pageSize).ToImmutableArray();
            var lastPage = false;

            if (peekNextPage.IsEmpty)
            {
                var nextLevels = _itemsByHierarchyLevel.Keys.Where(i => i > _currentLevel).ToImmutableArray();
                if (!nextLevels.IsEmpty)
                {
                    _currentLevel = nextLevels.First();
                    _currentLevelPage = DEFAULT_PAGE;
                }
                else 
                {
                    lastPage = true;
                }
            }

            return PagedResult<TContent>.Succeeded(
                levelPage, 
                _currentPage++, 
                _pageSize, 
                _totalCount, 
                lastPage);
        }
    }
}
