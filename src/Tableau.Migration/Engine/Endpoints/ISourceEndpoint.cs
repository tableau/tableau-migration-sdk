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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// <see cref="IMigrationEndpoint"/> interface for locations that serve as a source to load Tableau data from.
    /// This interface can be obtained through scoped dependency injection for the current in-progress migration.
    /// </summary>
    public interface ISourceEndpoint : IMigrationEndpoint
    {
        /// <summary>
        /// Pulls enough information to prepare and publish the content item.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <typeparam name="TPrepare">The preparation type.</typeparam>
        /// <param name="contentItem">The content item to pull.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the pull operation with the item to prepare and publish.</returns>
        Task<IResult<TPrepare>> PullAsync<TContent, TPrepare>(TContent contentItem, CancellationToken cancel)
            where TPrepare : class;

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
        /// Retrieves the encrypted keychains for the content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="destinationSiteInfo">The destination site information.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The operation result.</returns>
        Task<IResult<IEmbeddedCredentialKeychainResult>> RetrieveKeychainsAsync<TContent>(
           Guid contentItemId,
           IDestinationSiteInfo destinationSiteInfo,
           CancellationToken cancel)
             where TContent : IWithEmbeddedCredentials;


        /// <summary>
        /// Retrieves saved credentials for a specific user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="destinationSiteInfo">The destination site information.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The user's saved credentials.</returns>
        Task<IResult<IEmbeddedCredentialKeychainResult>> RetrieveUserSavedCredentialsAsync(
            Guid userId,
            IDestinationSiteInfo destinationSiteInfo,
            CancellationToken cancel);
    }
}