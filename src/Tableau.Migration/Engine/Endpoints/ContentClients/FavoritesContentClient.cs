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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Endpoints.ContentClients
{
    internal class FavoritesContentClient : ContentClientBase<IFavorite>, IFavoritesContentClient
    {
        private readonly IFavoritesApiClient _favoritesApiClient;
        private readonly IConfigReader _configReader;

        public FavoritesContentClient(
            IFavoritesApiClient favoritesApiClient,
            IConfigReader configReader,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer localizer)
            : base(loggerFactory.CreateLogger<FavoritesContentClient>(), localizer)
        {
            _favoritesApiClient = favoritesApiClient;
            _configReader = configReader;
        }

        /// <inheritdoc/>
        public async Task<IResult<IImmutableList<IFavorite>>> GetAllByUserAsync(IContentReference user, CancellationToken cancel)
        {
            var pager = _favoritesApiClient.GetPagerForUser(user, _configReader.Get<IUser>().BatchSize);
            return await pager.GetAllPagesAsync(cancel).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IResult> DeleteFavoriteForUserIdAsync(Guid userId, IFavorite favorite, CancellationToken cancel)
        {
            return await _favoritesApiClient.DeleteFavoriteAsync(userId, favorite.ContentType, favorite.Content.Id, cancel).ConfigureAwait(false);
        }
    }
}
