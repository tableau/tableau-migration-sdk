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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// <see cref="IMigrationEndpoint"/> interface for locations that serve as a destination to migrate Tableau data to.
    /// </summary>
    public interface IDestinationEndpoint : IMigrationEndpoint
    {
        /// <summary>
        /// Publishes a content item.
        /// </summary>
        /// <typeparam name="TPublish">The content type suitable for publishing.</typeparam>
        /// <typeparam name="TPublishResult">The publish result type.</typeparam>
        /// <param name="publishItem">The content item to publish.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The results of the publishing with a content reference of the newly published item.</returns>
        Task<IResult<TPublishResult>> PublishAsync<TPublish, TPublishResult>(TPublish publishItem, CancellationToken cancel)
            where TPublishResult : class, IContentReference;

        /// <summary>
        /// Publishes a batch of content items.
        /// </summary>
        /// <typeparam name="TPublish">The content type suitable for publishing.</typeparam>
        /// <param name="publishItems">The content items to publish.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The results of the publishing.</returns>
        Task<IResult> PublishBatchAsync<TPublish>(IEnumerable<TPublish> publishItems, CancellationToken cancel);

        /// <summary>
        /// Gets permissions for the content item.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="contentItem">The content item to get permissions for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the permissions operation.</returns>
        Task<IResult<IPermissions>> GetPermissionsAsync<TContent>(IContentReference contentItem, CancellationToken cancel)
            where TContent : IPermissionsContent;

        /// <summary>
        /// Gets permissions for the content item.
        /// </summary>
        /// <param name="type">The content type.</param>
        /// <param name="contentItem">The content item to get permissions for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the permissions operation.</returns>
        Task<IResult<IPermissions>> GetPermissionsAsync(Type type, IContentReference contentItem,
            CancellationToken cancel);

        /// <summary>
        /// Updates the permissions for the content item.
        /// </summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="permissions">The permissions of the content item.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IPermissions>> UpdatePermissionsAsync<TContent>(IContentReference contentItem, IPermissions permissions, CancellationToken cancel)
            where TContent : IPermissionsContent;

        /// <summary>
        /// Updates the permissions for the content item.
        /// </summary>
        /// <param name="type">Type of content item</param>
        /// <param name="contentItem">The content item.</param>
        /// <param name="permissions">The permissions of the content item.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IPermissions>> UpdatePermissionsAsync(Type type, IContentReference contentItem,
            IPermissions permissions, CancellationToken cancel);

        /// <summary>
        /// Updates the tags for the content item.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="contentItem">The content item to update tags for.</param>
        /// <param name="tags">The tags to apply.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the tag update operation.</returns>
        Task<IResult> UpdateTagsAsync<TContent>(IContentReference contentItem, IEnumerable<ITag> tags, CancellationToken cancel)
            where TContent : IWithTags;

        /// <summary>
        /// Updates the owner for the content item.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="contentItem">The content item to update ownership for.</param>
        /// <param name="owner">The new owner.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the owner update operation.</returns>
        Task<IResult> UpdateOwnerAsync<TContent>(IContentReference contentItem, IContentReference owner, CancellationToken cancel)
            where TContent : IWithOwner;

        /// <summary>
        /// List the content item's connections.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>An immutable list of connections.</returns>
        Task<IResult<ImmutableList<IConnection>>> ListConnectionsAsync<TContent>(
            Guid contentItemId,
            CancellationToken cancel)
            where TContent : IWithConnections;

        /// <summary>
        /// Update the connection on the data source.
        /// </summary>       
        /// <param name="contentItemId">The ID of the content item to update connections for.</param>
        /// <param name="connectionId">The ID of the connection to be updated.</param>
        /// <param name="options">The update connetion options.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the connection update operation.</returns>
        Task<IResult<IConnection>> UpdateConnectionAsync<TContent>(
            Guid contentItemId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel)
            where TContent : IWithConnections;

        /// <summary>
        /// Update the settings of a site.
        /// </summary>
        /// <param name="newSiteSettings">The site settings to update.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the site update operation.</returns>
        Task<IResult<ISite>> UpdateSiteSettingsAsync(ISiteSettingsUpdate newSiteSettings, CancellationToken cancel);

        /// <summary>
        /// Update the custom view's default users.
        /// </summary>
        /// <param name="id">The ID of the custom view.</param>
        /// <param name="users">The list of users who have the custom view as their default.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the default user list update.</returns>
        Task<IResult<IImmutableList<ICustomViewAsUserDefaultViewResult>>> SetCustomViewDefaultUsersAsync(
            Guid id,
            IEnumerable<IContentReference> users,
            CancellationToken cancel);
    }
}
