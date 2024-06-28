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
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Permissions
{
    /// <summary>
    /// Interface for API client project default permissions operations.
    /// </summary>
    public interface IDefaultPermissionsApiClient
    {
        /// <summary>
        /// Gets the content type's default permissions for the project with the specified ID.
        /// </summary>
        /// <param name="contentTypeUrlSegment">The permissions' content type URL segment.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IPermissions>> GetPermissionsAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            CancellationToken cancel);

        /// <summary>
        /// Gets the default permissions for the project with the specified ID.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The permissions result with content type <see cref="IPermissions"/>.</returns>
        Task<IResult<IImmutableDictionary<string, IPermissions>>> GetAllPermissionsAsync(
            Guid projectId,
            CancellationToken cancel);

        /// <summary>
        /// Creates the content type's default permissions for the project with the specified ID.
        /// </summary>
        /// <param name="contentTypeUrlSegment">The permissions' content type URL segment.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="permissions">The permissions of the content item.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IPermissions>> CreatePermissionsAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            IPermissions permissions,
            CancellationToken cancel);

        /// <summary>
        /// Remove a content type's default <see cref="ICapability"/> for a user/group in a project.
        /// </summary>
        /// <param name="contentTypeUrlSegment">The permissions' content type URL segment.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="granteeId">The id of the permissions grantee. This is either a user or group.</param>
        /// <param name="granteeType">The type of the permissions grantee.</param>
        /// <param name="capability">The object containing the capability name and mode.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> DeleteCapabilityAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            Guid granteeId,
            GranteeType granteeType,
            ICapability capability,
            CancellationToken cancel);

        /// <summary>
        /// Remove all content type's default <paramref name="permissions"/> for a project.
        /// </summary>
        /// <param name="contentTypeUrlSegment">The permissions' content type URL segment.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="permissions"></param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> DeleteAllPermissionsAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            IPermissions permissions,
            CancellationToken cancel);

        /// <summary>
        /// Updates the content type's default permissions for the project with the specified ID.
        /// </summary>
        /// <param name="contentTypeUrlSegment">The permissions' content type URL segment.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="permissions">The permissions of the content item.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IPermissions>> UpdatePermissionsAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            IPermissions permissions,
            CancellationToken cancel);

        /// <summary>
        /// Updates the content types' default permissions for the project with the specified ID.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="permissions">The permissions of the content items, keyed by the content type's URL segment.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IImmutableDictionary<string, IPermissions>>> UpdateAllPermissionsAsync(
            Guid projectId,
            IReadOnlyDictionary<string, IPermissions> permissions,
            CancellationToken cancel);
    }
}