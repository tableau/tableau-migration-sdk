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

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal sealed class RestAddFavoriteResponseBuilder : RestGetFavoritesForUserResponseBuilder
    {
        public RestAddFavoriteResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer)
        { }

        protected override ValueTask<(FavoritesResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var addRequest = request.GetTableauServerRequest<AddFavoriteRequest>();
            if (addRequest?.Favorite is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Content cannot be null.", string.Empty);
            }

            var userId = request.GetRequestIdFromUri();
            if (!Data.UserFavorites.TryGetValue(userId, out var userFavorites))
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 002, "User not found", string.Empty);
            }

            var contentType = addRequest.Favorite.GetContentType();
            var contentId = addRequest.Favorite.GetContentId();
            var label = addRequest.Favorite.Label ?? string.Empty;

            object? contentItem = null;
            int notFoundSubCode = 0;
            switch (contentType)
            {
                case Content.FavoriteContentType.DataSource:
                    contentItem = Data.DataSources.FirstOrDefault(ds => ds.Id == contentId);
                    notFoundSubCode = 011;
                    break;
                case Content.FavoriteContentType.Flow:
                    contentItem = Data.Flows.FirstOrDefault(f => f.Id == contentId);
                    notFoundSubCode = 027;
                    break;
                case Content.FavoriteContentType.Project:
                    contentItem = Data.Projects.FirstOrDefault(p => p.Id == contentId);
                    notFoundSubCode = 005;
                    break;
                case Content.FavoriteContentType.View:
                    contentItem = Data.Workbooks.FirstOrDefault(w => w.Views.Any(v => v.Id == contentId));
                    notFoundSubCode = 011;
                    break;
                case Content.FavoriteContentType.Workbook:
                    contentItem = Data.Workbooks.FirstOrDefault(w => w.Id == contentId);
                    notFoundSubCode = 006;
                    break;
                case Content.FavoriteContentType.Collection:
                    contentItem = Data.Collections.FirstOrDefault(w => w.Id == contentId);
                    notFoundSubCode = 011;
                    break;
                default:
                    break;
            }

            if (contentItem is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, notFoundSubCode, "Content item not found", string.Empty);
            }

            userFavorites.AddOrUpdate((contentType, contentId), label, (key, oldValue) => label);

            return base.BuildResponseAsync(userId);
        }
    }
}
