//
//  Copyright (c) 2024, Salesforce, Inc.
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
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// <see cref="IMigrationEndpoint"/> impelementation that uses Tableau Server/Cloud APIs.
    /// </summary>
    public abstract class TableauApiEndpointBase : IMigrationApiEndpoint
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private IAsyncDisposableResult<ISitesApiClient>? _signInResult;

        /// <summary>
        /// The per-endpoint dependency injection scope.
        /// </summary>
        protected readonly AsyncServiceScope EndpointScope;

        /// <summary>
        /// The server-level API client.
        /// </summary>
        protected readonly IApiClient ServerApi;

        /// <summary>
        /// Gets the site-level API client.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the endpoint has not been initialized or site sign in failed.</exception>
        public ISitesApiClient SiteApi
        {
            get
            {
                if (_signInResult is null)
                {
                    throw new InvalidOperationException(_localizer[SharedResourceKeys.ApiEndpointNotInitializedError]);
                }
                else if (_signInResult.Value is null)
                {
                    throw new InvalidOperationException(_localizer[SharedResourceKeys.ApiEndpointDoesnotHaveValidSiteError]);
                }

                return _signInResult.Value;
            }
        }

        /// <summary>
        /// Creates a new <see cref="TableauApiEndpointBase"/> object.
        /// </summary>
        /// <param name="serviceScopeFactory">A service scope factory to define an API client scope with.</param>
        /// <param name="config">The configuration options for connecting to the endpoint APIs.</param>
        /// <param name="finderFactory">The content finder factory to supply to the API client.</param>
        /// <param name="fileStore">The file store to use.</param>
        /// <param name="localizer">A string localizer.</param>
        public TableauApiEndpointBase(IServiceScopeFactory serviceScopeFactory,
            ITableauApiEndpointConfiguration config,
            IContentReferenceFinderFactory finderFactory,
            IContentFileStore fileStore, 
            ISharedResourcesLocalizer localizer)
        {
            EndpointScope = serviceScopeFactory.CreateAsyncScope();

            var apiClientFactory = EndpointScope.ServiceProvider.GetRequiredService<IScopedApiClientFactory>();

            ServerApi = apiClientFactory.Initialize(config.SiteConnectionConfiguration, finderFactory, fileStore);
            _localizer = localizer;
        }

        #region - IAsyncDisposable Implementation -

        /// <summary>
        /// Disposes the result's value.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_signInResult is not null)
                await _signInResult.DisposeAsync().ConfigureAwait(false);

            await EndpointScope.DisposeAsync().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region - IMigrationEndpoint Implementation -

        /// <inheritdoc />
        public async Task<IResult> InitializeAsync(CancellationToken cancel)
        {
            _signInResult = await ServerApi.SignInAsync(cancel).ConfigureAwait(false);
            return _signInResult;
        }

        /// <inheritdoc />
        public IPager<TContent> GetPager<TContent>(int pageSize)
        {
            var listApi = SiteApi.GetListApiClient<TContent>();
            return listApi.GetPager(pageSize);
        }

        /// <inheritdoc />
        public async Task<IResult<IServerSession>> GetSessionAsync(CancellationToken cancel)
            => await ServerApi.GetCurrentServerSessionAsync(cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IResult<IPermissions>> GetPermissionsAsync<TContent>(IContentReference contentItem, CancellationToken cancel)
            where TContent : IPermissionsContent
        {
            return await GetPermissionsAsync(typeof(TContent), contentItem, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IPermissions>> GetPermissionsAsync(Type type, IContentReference contentItem, CancellationToken cancel)
        {
            var apiClient = SiteApi.GetPermissionsApiClient(type);
            return await apiClient.GetPermissionsAsync(contentItem.Id, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<ImmutableList<IConnection>>> ListConnectionsAsync<TContent>(
            Guid contentItemId,
            CancellationToken cancel)
            where TContent : IWithConnections
        {
            var apiClient = SiteApi.GetConnectionsApiClient<TContent>();
            return await apiClient.GetConnectionsAsync(contentItemId, cancel).ConfigureAwait(false);
        }

        #endregion
    }
}
