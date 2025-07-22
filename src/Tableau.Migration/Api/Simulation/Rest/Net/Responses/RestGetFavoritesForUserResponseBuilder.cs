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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Content;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest.Paging;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestGetFavoritesForUserResponseBuilder : RestResponseBuilderBase<FavoritesResponse>
    {
        private readonly IEntityResponsePager<KeyValuePair<(FavoriteContentType, Guid), string>> _pager;

        public RestGetFavoritesForUserResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        {
            _pager = new RestEntityResponsePager<KeyValuePair<(FavoriteContentType, Guid), string>>();
        }

        protected FavoritesResponse.FavoriteType BuildFavoriteResponseItem(FavoriteContentType contentType, Guid contentId, string label)
        {
            var item = new FavoritesResponse.FavoriteType()
            {
                Label = label,
            };

            switch (contentType)
            {
                case FavoriteContentType.DataSource:
                    if (Data.DataSources.SingleOrDefault(ds => ds.Id == contentId) is not null)
                    {
                        item.DataSource = new() { Id = contentId };
                    }
                    break;

                case FavoriteContentType.Flow:
                    // Use stub value until flows are simulated.
                    item.Flow = new()
                    {
                        Id = contentId,
                        Project = new()
                        {
                            Id = Data.Projects.First().Id
                        }
                    };
                    break;

                case FavoriteContentType.Project:
                    if (Data.Projects.SingleOrDefault(p => p.Id == contentId) is not null)
                    {
                        item.Project = new() { Id = contentId };
                    }
                    break;

                case FavoriteContentType.View:
                    var wb = Data.Workbooks.SingleOrDefault(w => w.Views.Any(v => v.Id == contentId));
                    if (wb is not null)
                    {
                        var view = wb.Views.Single(v => v.Id == contentId);
                        item.View = new()
                        {
                            Id = contentId,
                            ContentUrl = view.ContentUrl,
                            Name = view.Name,
                            Workbook = new()
                            {
                                Id = wb.Id
                            }
                        };
                    }
                    break;

                case FavoriteContentType.Workbook:
                    if (Data.Workbooks.SingleOrDefault(wb => wb.Id == contentId) is not null)
                    {
                        item.Workbook = new() { Id = contentId };
                    }
                    break;

                case FavoriteContentType.Collection:
                    if (Data.Collections.SingleOrDefault(coll => coll.Id == contentId) is not null)
                    {
                        item.Collection = new() { Id = contentId };
                    }
                    break;
                default:
                    break;
            }

            return item;
        }

        protected ValueTask<(FavoritesResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(Guid userId,
            Page? pageOptions = null)
        {
            if (!Data.UserFavorites.TryGetValue(userId, out var items) || items.IsNullOrEmpty())
            {
                return ValueTask.FromResult((new FavoritesResponse(), HttpStatusCode.OK));
            }

            var itemArray = items
                .Where(kvp => kvp.Key != default && kvp.Value != null)
                .ToImmutableArray();

            ImmutableArray<KeyValuePair<(FavoriteContentType ContentType, Guid Id), string>> pagedItems =
                pageOptions is null ? itemArray : _pager.GetPage(itemArray, pageOptions.Value);

            var response = new FavoritesResponse
            {
                Items = pagedItems.Select(i => BuildFavoriteResponseItem(i.Key.ContentType, i.Key.Id, i.Value)).ToArray()
            };

            return ValueTask.FromResult((response, HttpStatusCode.OK));
        }

        protected override ValueTask<(FavoritesResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var userId = request.GetRequestIdFromUri();
            if (!Data.UserFavorites.TryGetValue(userId, out var userFavorites))
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 002, "User not found", string.Empty);
            }

            var pageOptions = _pager.GetPageOptions(request);

            return BuildResponseAsync(userId, pageOptions);
        }
    }
}
