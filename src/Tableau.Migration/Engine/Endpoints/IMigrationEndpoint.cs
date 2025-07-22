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
using Tableau.Migration.Engine.Caching;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that represents a location to move Tableau data to or from.
    /// </summary>
    public interface IMigrationEndpoint : IAsyncDisposable
    {
        /// <summary>
        /// Performs pre-migration initialization.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>An awaitable task with the initialization result.</returns>
        Task<IResult> InitializeAsync(CancellationToken cancel);

        /// <summary>
        /// Gets a pager to list all the content the user has access to.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="pageSize">The page size to use.</param>
        /// <returns>A pager to list content with.</returns>
        IPager<TContent> GetPager<TContent>(int pageSize);

        /// <summary>
        /// Gets the current server session information.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>An awaitable task with the server session result.</returns>
        Task<IResult<IServerSession>> GetSessionAsync(CancellationToken cancel);

        #region - Caches -

        /// <summary>
        /// Gets an migration cache that belongs to the endpoint scope.
        /// </summary>
        /// <typeparam name="TCache">The cache type.</typeparam>
        /// <typeparam name="TKey">The cache key type.</typeparam>
        /// <typeparam name="TValue">The cache value type.</typeparam>
        /// <returns>The cache.</returns>
        TCache GetEndpointCache<TCache, TKey, TValue>()
            where TCache : IMigrationCache<TKey, TValue>
            where TKey : notnull
            where TValue : class;

        /// <summary>
        /// Gets the cache of views in the endpoint scope.
        /// </summary>
        /// <returns>The view cache.</returns>
        public IEndpointViewCache GetViewCache()
            => GetEndpointCache<IEndpointViewCache, Guid, IView>();

        /// <summary>
        /// Gets the cache of views by workbook in the endpoint scope.
        /// </summary>
        /// <returns>The workbook views cache.</returns>
        public IEndpointWorkbookViewsCache GetWorkbookViewsCache()
            => GetEndpointCache<IEndpointWorkbookViewsCache, Guid, IImmutableList<IView>>();

        #endregion

        #region - Content Clients -

        /// <summary>
        /// Gets the current site information.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>An awaitable task with the site result.</returns>
        Task<IResult<ISite>> GetCurrentSiteAsync(CancellationToken cancel);

        /// <summary>
        /// Gets a content client for the given content type.
        /// </summary>
        /// <typeparam name="TContentClient">The content client type.</typeparam>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The content client.</returns>
        TContentClient GetContentClient<TContentClient, TContent>()
            where TContentClient : IContentClient<TContent>;

        /// <summary>
        /// Gets a favorites content client.
        /// </summary>
        /// <returns>The content client.</returns>
        public IFavoritesContentClient GetFavoritesContentClient() => GetContentClient<IFavoritesContentClient, IFavorite>();

        /// <summary>
        /// Gets a views content client.
        /// </summary>
        /// <returns>The content client.</returns>
        public IViewsContentClient GetViewsContentClient() => GetContentClient<IViewsContentClient, IView>();

        /// <summary>
        /// Gets a workbook content client.
        /// </summary>
        /// <returns>The content client.</returns>
        public IWorkbooksContentClient GetWorkbookContentClient() => GetContentClient<IWorkbooksContentClient, IWorkbook>();

        #endregion
    }
}
