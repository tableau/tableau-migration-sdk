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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class FavoritesApiClient : ContentApiClientBase, IFavoritesApiClient
    {
        private readonly IHttpContentSerializer _serializer;
        private readonly IConfigReader _configReader;

        public FavoritesApiClient(IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            IHttpContentSerializer serializer,
            IConfigReader configReader,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _serializer = serializer;
            _configReader = configReader;
        }

        private async Task<IFavorite?> CreateFavoriteAsync(IContentReference user, FavoritesResponse.FavoriteType item, CancellationToken cancel)
        {
            var contentType = item.GetContentType();

            IContentReference? contentReference;
            switch (contentType)
            {
                case FavoriteContentType.Flow:
                    /*
                     * Build a reference without a content finder since we currently don't support flows.
                     * These favorites will be filtered by a default filter.
                     */
                    Guard.AgainstNull(item.Flow, () => item.Flow);
                    Guard.AgainstNull(item.Flow.Project, () => item.Flow.Project);

                    var project = await FindProjectByIdAsync(item.Flow.Project.Id, throwIfNotFound: false, cancel).ConfigureAwait(false);
                    if (project == null)
                    {
                        LogWarningForMissingContent("project", item.Flow.Project.Id);
                        return null;
                    }
                    contentReference = new ContentReferenceStub(item.Flow.Id, string.Empty, project.Location.Append(item.Flow.Name ?? string.Empty), item.Flow.Name ?? string.Empty);
                    break;
                case FavoriteContentType.View:
                    /*
                     * Find the workbook then build the view reference,
                     * since we don't have a view content finder.
                     */
                    Guard.AgainstNull(item.View, () => item.View);
                    Guard.AgainstNull(item.View.Workbook, () => item.View.Workbook);

                    var workbook = await FindWorkbookByIdAsync(item.View.Workbook.Id, throwIfNotFound: false, cancel).ConfigureAwait(false);
                    if (workbook == null)
                    {
                        LogWarningForMissingContent("workbook", item.View.Workbook.Id);
                        return null;
                    }
                    contentReference = new ContentReferenceStub(item.View.Id, item.View.ContentUrl ?? string.Empty, workbook.Location.Append(item.View.Name ?? string.Empty), item.View.Name ?? string.Empty);
                    break;
                case FavoriteContentType.Collection:
                    /*
                    * Build a reference without a content finder since we currently don't support collections.
                    * These favorites will be filtered by a default filter.
                    */
                    Guard.AgainstNull(item.Collection, () => item.Collection);
                    contentReference = new ContentReferenceStub(
                        item.Collection.Id, string.Empty, user.Location.Append(item.Collection.Name ?? string.Empty),
                        item.Collection.Name ?? string.Empty);
                    break;
                case FavoriteContentType.Unknown:
                    Logger.LogWarning(SharedResourcesLocalizer[SharedResourceKeys.FavoriteContentTypeNotSupportedWarning],
                        item.Label, contentType);
                    return null;
                default:
                    var finder = ContentFinderFactory.ForFavoriteContentType(contentType);

                    contentReference = await finder.FindByIdAsync(item.GetContentId(), cancel).ConfigureAwait(false);
                    if (contentReference == null)
                    {
                        LogWarningForMissingContent(contentType.ToString(), item.GetContentId());
                        return null;
                    }
                    break;
            }

            return new Favorite(user, contentType, contentReference, item.Label);

            void LogWarningForMissingContent(string requiredContentType, Guid contentId)
                => Logger.LogWarning(SharedResourcesLocalizer[SharedResourceKeys.FavoriteSkippedMissingContentWarning],
                    contentType, contentId, requiredContentType);
        }

        private async Task<IImmutableList<IFavorite>> CreateFavoritesAsync(IContentReference user, FavoritesResponse response, CancellationToken cancel)
        {
            var results = ImmutableArray.CreateBuilder<IFavorite>(response.Items.Length);

            foreach (var item in response.Items)
            {
                var favorite = await CreateFavoriteAsync(user, item, cancel).ConfigureAwait(false);

                if (favorite is not null)
                {
                    results.Add(favorite);
                }
            }

            return results.ToImmutable();
        }

        #region - IFavoritesApiClient Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IFavorite>> GetFavoritesForUserAsync(IContentReference user, int pageNumber, int pageSize, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"/favorites/{user.Id.ToUrlSegment()}")
                .WithPage(pageNumber, pageSize)
                .ForGetRequest()
                .SendAsync<FavoritesResponse>(cancel)
                .ToPagedResultAsync(async (r, c) => await CreateFavoritesAsync(user, r, c).ConfigureAwait(false), SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public IPager<IFavorite> GetPagerForUser(IContentReference user, int pageSize)
            => new CallbackPager<IFavorite>((pn, ps, c) => GetFavoritesForUserAsync(user, pn, ps, c), pageSize);

        /// <inheritdoc />
        public async Task<IResult<IImmutableList<IFavorite>>> AddFavoriteAsync(IContentReference user, string label, FavoriteContentType contentType, Guid contentItemId, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"/favorites/{user.Id.ToUrlSegment()}")
                .ForPutRequest()
                .WithXmlContent(new AddFavoriteRequest(label, contentType, contentItemId))
                .SendAsync<FavoritesResponse>(cancel)
                .ToResultAsync(async (r, c) => await CreateFavoritesAsync(user, r, c).ConfigureAwait(false), SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        internal static string GetContentTypeUrlSegment(FavoriteContentType contentType)
        {
            return contentType switch
            {
                FavoriteContentType.DataSource => "datasources",
                FavoriteContentType.Flow => "flows",
                FavoriteContentType.Project => "projects",
                FavoriteContentType.View => "views",
                FavoriteContentType.Workbook => "workbooks",
                FavoriteContentType.Collection => "collections",
                _ => throw new ArgumentException($"URL segment of {contentType} favorites is not supported.")
            };
        }

        /// <inheritdoc />
        public async Task<IResult> DeleteFavoriteAsync(Guid userId, FavoriteContentType contentType, Guid contentItemId, CancellationToken cancel)
        {
            var contentUrlSegment = GetContentTypeUrlSegment(contentType);

            var result = await RestRequestBuilderFactory
                .CreateUri($"/favorites/{userId.ToUrlSegment()}/{contentUrlSegment}/{contentItemId.ToUrlSegment()}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region - IPagedListApiClient<IFavorite> Implementation -

        /// <inheritdoc />
        public IPager<IFavorite> GetPager(int pageSize)
            => new FavoritesApiListPager(this, ContentFinderFactory, _configReader, pageSize);

        #endregion

        #region - IPublishApiClient<IFavorite> Implmentation -

        /// <inheritdoc />
        public async Task<IResult<IFavorite>> PublishAsync(IFavorite item, CancellationToken cancel)
        {
            var publishResult = await AddFavoriteAsync(item.User, item.Label, item.ContentType, item.Content.Id, cancel).ConfigureAwait(false);
            if (!publishResult.Success)
            {
                return publishResult.CastFailure<IFavorite>();
            }

            var favoriteResult = publishResult.Value.Single(f => f.Content.Id == item.Content.Id);
            return Result<IFavorite>.Succeeded(favoriteResult);
        }

        #endregion
    }
}
