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

namespace Tableau.Migration.Engine.Endpoints.ContentClients
{
    /// <summary>
    /// Interface for a client that can interact with favorites.
    /// </summary>
    public interface IFavoritesContentClient : IContentClient<IFavorite>
    {
        /// <summary>
        /// Get all favorites for a user by the content reference
        /// </summary>
        /// <param name="user">The <see cref="IContentReference"/> of the user.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>List of all favorites of a user.</returns>
        public Task<IResult<IImmutableList<IFavorite>>> GetAllByUserAsync(IContentReference user, CancellationToken cancel);

        /// <summary>
        /// Delete favorite for a user 
        /// </summary>
        /// <param name="userId">UserId of the user.</param>
        /// <param name="favorite">The favorite item to delete.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>Result of the operation.</returns>
        public Task<IResult> DeleteFavoriteForUserIdAsync(Guid userId, IFavorite favorite, CancellationToken cancel);
    }
}
