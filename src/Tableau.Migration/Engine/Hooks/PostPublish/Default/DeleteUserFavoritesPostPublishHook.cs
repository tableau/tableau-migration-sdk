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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.ContentClients;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    internal class DeleteUserFavoritesPostPublishHook : ContentItemPostPublishHookBase<IUser>
    {
        private readonly IFavoritesContentClient _favoritesContentClient;
        private readonly IConfigReader _configReader;

        public DeleteUserFavoritesPostPublishHook(IDestinationEndpoint destinationEndpoint, IConfigReader configReader)
        {
            _favoritesContentClient = destinationEndpoint.GetFavoritesContentClient();
            _configReader = configReader;
        }

        public override async Task<ContentItemPostPublishContext<IUser, IUser>?> ExecuteAsync(ContentItemPostPublishContext<IUser, IUser> ctx, CancellationToken cancel)
        {
            if (_configReader.Get<IFavorite>().OverwriteUserFavoritesEnabled == false)
            {
                // If the configuration is set to not overwrite user favorites
                // Short circuit the hook and return null
                return null;
            }

            // Get all the favorites of the user
            var favorites = await _favoritesContentClient.GetAllByUserAsync(ctx.DestinationItem, cancel).ConfigureAwait(false);

            if (!favorites.Success)
            {
                throw new Exception($"Unable to retrieve destination favorites for user {ctx.DestinationItem.Name}.");
            }

            // Delete all the favorites, aggregate exceptions if any
            List<Exception> exceptions = new List<Exception>();
            foreach (var fav in favorites.Value)
            {
                var deleteResult = await _favoritesContentClient.DeleteFavoriteForUserIdAsync(ctx.DestinationItem.Id, fav, cancel).ConfigureAwait(false);

                if (!deleteResult.Success)
                {
                    exceptions.Add(new Exception($"Unable to delete favorite {fav.Content.Id} for user {ctx.DestinationItem.Name}."));
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("Failed to delete some favorites.", exceptions);
            }

            // No changes, return null
            return null;
        }
    }
}
