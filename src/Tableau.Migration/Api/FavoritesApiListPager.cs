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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    internal sealed class FavoritesApiListPager : MemoryPagerBase<IFavorite>
    {
        private readonly IFavoritesApiClient _apiClient;
        private readonly IContentReferenceFinder _userFinder;
        private readonly IConfigReader _configReader;

        public FavoritesApiListPager(IFavoritesApiClient apiClient, IContentReferenceFinderFactory finderFactory, IConfigReader configReader, int pageSize)
            : base(pageSize)
        {
            _apiClient = apiClient;
            _userFinder = finderFactory.ForContentType<IUser>();
            _configReader = configReader;
        }

        protected override async Task<IResult<IReadOnlyCollection<IFavorite>>> LoadItemsAsync(CancellationToken cancel)
        {
            /*
             * We eager load favorites for all users so we can provide an accurate total count.
             * Lazy loading would cause jitters in the total count.
             */
            var results = ImmutableArray.CreateBuilder<IFavorite>();
            var favoriteBatchSize = _configReader.Get<IFavorite>().BatchSize;

            var users = await _userFinder.FindAllAsync(cancel).ConfigureAwait(false);
            foreach (var user in users)
            {
                var userPager = _apiClient.GetPagerForUser(user, favoriteBatchSize);

                var userFavoriteResult = await userPager.GetAllPagesAsync(cancel).ConfigureAwait(false);
                if (!userFavoriteResult.Success)
                {
                    return userFavoriteResult.CastFailure<IReadOnlyCollection<IFavorite>>();
                }

                results.AddRange(userFavoriteResult.Value);
            }

            return Result<IReadOnlyCollection<IFavorite>>.Succeeded(results);
        }
    }
}
