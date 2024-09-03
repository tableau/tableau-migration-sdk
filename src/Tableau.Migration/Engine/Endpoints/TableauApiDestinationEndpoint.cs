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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// <see cref="IDestinationEndpoint"/> implementation that uses Tableau Server/Cloud APIs.
    /// </summary>
    public class TableauApiDestinationEndpoint : TableauApiEndpointBase, IDestinationApiEndpoint
    {
        /// <summary>
        /// Creates a new <see cref="TableauApiDestinationEndpoint"/> object.
        /// </summary>
        /// <param name="serviceScopeFactory">A service scope factory to define an API client scope with.</param>
        /// <param name="config">The configuration options for connecting to the destination endpoint APIs.</param>
        /// <param name="finderFactory">A destination finder factory.</param>
        /// <param name="fileStore">The file store to use.</param>
        /// <param name="localizer">A string localizer.</param>
        public TableauApiDestinationEndpoint(IServiceScopeFactory serviceScopeFactory,
            ITableauApiEndpointConfiguration config,
            IDestinationContentReferenceFinderFactory finderFactory,
            IContentFileStore fileStore,
            ISharedResourcesLocalizer localizer)
            : base(serviceScopeFactory, config, finderFactory, fileStore, localizer)
        { }

        /// <inheritdoc />
        public async Task<IResult<TPublishResult>> PublishAsync<TPublish, TPublishResult>(TPublish publishItem, CancellationToken cancel)
            where TPublishResult : class, IContentReference
        {
            var publishApiClient = SiteApi.GetPublishApiClient<TPublish, TPublishResult>();
            return await publishApiClient.PublishAsync(publishItem, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult> PublishBatchAsync<TPublish>(IEnumerable<TPublish> publishItems, CancellationToken cancel)
        {
            var publishApiClient = SiteApi.GetBatchPublishApiClient<TPublish>();
            return await publishApiClient.PublishBatchAsync(publishItems, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IPermissions>> UpdatePermissionsAsync<TContent>(IContentReference contentItem, IPermissions permissions, CancellationToken cancel)
            where TContent : IPermissionsContent
        {
            return await UpdatePermissionsAsync(typeof(TContent), contentItem, permissions, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IPermissions>> UpdatePermissionsAsync(Type type, IContentReference contentItem, IPermissions permissions, CancellationToken cancel)
        {
            var apiClient = SiteApi.GetPermissionsApiClient(type);
            return await apiClient.UpdatePermissionsAsync(contentItem.Id, permissions, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult> UpdateTagsAsync<TContent>(IContentReference contentItem, IEnumerable<ITag> tags, CancellationToken cancel)
            where TContent : IWithTags
        {
            var apiClient = SiteApi.GetTagsApiClient<TContent>();
            return await apiClient.UpdateTagsAsync(contentItem.Id, tags, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult> UpdateOwnerAsync<TContent>(IContentReference contentItem, IContentReference owner, CancellationToken cancel)
            where TContent : IWithOwner
        {
            var apiClient = SiteApi.GetOwnershipApiClient<TContent>();
            return await apiClient.ChangeOwnerAsync(contentItem.Id, owner.Id, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IConnection>> UpdateConnectionAsync<TContent>(
            Guid contentItemId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel) where TContent : IWithConnections
        {
            var apiClient = SiteApi.GetConnectionsApiClient<TContent>();
            return await apiClient.UpdateConnectionAsync(contentItemId, connectionId, options, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<ISite>> UpdateSiteSettingsAsync(ISiteSettingsUpdate newSiteSettings, CancellationToken cancel)
        {
            return await SiteApi.UpdateSiteAsync(newSiteSettings, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IImmutableList<ICustomViewAsUserDefaultViewResult>>> SetCustomViewDefaultUsersAsync(
            Guid id,
            IEnumerable<IContentReference> users,
            CancellationToken cancel)
        {
            return await SiteApi.CustomViews
                .SetCustomViewDefaultUsersAsync(id, users, cancel)
                .ConfigureAwait(false);
        }
    }
}
