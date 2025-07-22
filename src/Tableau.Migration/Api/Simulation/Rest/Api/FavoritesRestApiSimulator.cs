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
using System.Linq;
using System.Net;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API favorites methods.
    /// </summary>
    public sealed class FavoritesRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated get favorites for user API method.
        /// </summary>
        public MethodSimulator GetFavoritesForUser { get; }

        /// <summary>
        /// Gets the simulated add favorite API method.
        /// </summary>
        public MethodSimulator AddFavorite { get; }

        /// <summary>
        /// Gets the simulated delete favorite API methods.
        /// </summary>
        public ImmutableArray<MethodSimulator> DeleteFavorites { get; }

        /// <summary>
        /// Creates a new <see cref="FavoritesRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public FavoritesRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            GetFavoritesForUser = simulator.SetupRestGet(SiteEntityUrl("favorites"),
                new RestGetFavoritesForUserResponseBuilder(simulator.Data, simulator.Serializer));

            AddFavorite = simulator.SetupRestPut(SiteEntityUrl("favorites"),
                new RestAddFavoriteResponseBuilder(simulator.Data, simulator.Serializer));

            var deleteResponseBuilder = new RestDeleteResponseBuilder(simulator.Data, static (data, request) =>
            {
                var userId = request.GetIdAfterSegment("favorites");
                if (userId is null || !data.UserFavorites.TryGetValue(userId.Value, out var userFavorites))
                {
                    return HttpStatusCode.NotFound;
                }

                var contentType = request.GetNextToLastSegment()?.ToLower() switch
                {
                    "datasources" => FavoriteContentType.DataSource,
                    "flows" => FavoriteContentType.Flow,
                    "projects" => FavoriteContentType.Project,
                    "views" => FavoriteContentType.View,
                    "workbooks" => FavoriteContentType.Workbook,
                    "collections" => FavoriteContentType.Collection,
                    _ => FavoriteContentType.Unknown
                };

                if (contentType is FavoriteContentType.Unknown)
                {
                    return HttpStatusCode.BadRequest;
                }

                var contentId = request.GetRequestIdFromUri();
                var key = (contentType, contentId);
                if (userFavorites.TryRemove(key, out var val))
                {
                    return HttpStatusCode.NoContent;
                }
                else
                {
                    return HttpStatusCode.NotFound;
                }
            }, simulator.Serializer);

            DeleteFavorites = Enum.GetValues<FavoriteContentType>()
                .Where(t => t is not FavoriteContentType.Unknown)
                .Select(t => simulator.SetupRestDelete(SiteEntityUrl($"favorites/{UserId}/{FavoritesApiClient.GetContentTypeUrlSegment(t)}"), deleteResponseBuilder))
                .ToImmutableArray();
        }
    }
}
