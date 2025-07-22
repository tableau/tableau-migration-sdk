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
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client favorites operations.
    /// </summary>
    public interface IFavoritesApiClient : IContentApiClient, IPagedListApiClient<IFavorite>, IPublishApiClient<IFavorite>
    {
        /// <summary>
        /// Gets the favorites for a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A list of favorites for the given user.</returns>
        Task<IPagedResult<IFavorite>> GetFavoritesForUserAsync(IContentReference user, int pageNumber, int pageSize, CancellationToken cancel);

        /// <summary>
        /// Gets a pager to list all the favorites a user has access to.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pageSize">The page size to use.</param>
        /// <returns>A pager to list favorites with.</returns>
        IPager<IFavorite> GetPagerForUser(IContentReference user, int pageSize);

        /// <summary>
        /// Adds a favorite.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="label">The favorite label.</param>
        /// <param name="contentType">The content reference type.</param>
        /// <param name="contentItemId">The content reference ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A list of favorites for the given user.</returns>
        Task<IResult<IImmutableList<IFavorite>>> AddFavoriteAsync(IContentReference user, string label, FavoriteContentType contentType, Guid contentItemId, CancellationToken cancel);

        /// <summary>
        /// Deletes a favorite.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="contentType">The content reference type.</param>
        /// <param name="contentItemId">The content reference ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result.</returns>
        Task<IResult> DeleteFavoriteAsync(Guid userId, FavoriteContentType contentType, Guid contentItemId, CancellationToken cancel);        
    }
}
