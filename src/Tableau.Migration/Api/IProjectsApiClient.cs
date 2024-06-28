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
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client project operations.
    /// </summary>
    public interface IProjectsApiClient : IContentApiClient,
        IPagedListApiClient<IProject>, IPublishApiClient<IProject>, IPermissionsContentApiClient,
        IOwnershipApiClient
    {
        /// <summary>
        /// Creates a project.
        /// </summary>
        /// <param name="options">The new project's details.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The newly created project.</returns>
        Task<IResult<IProject>> CreateProjectAsync(
            ICreateProjectOptions options,
            CancellationToken cancel);

        /// <summary>
        /// Gets the project's default permissions.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        Task<IResult<IImmutableDictionary<string, IPermissions>>> GetAllDefaultPermissionsAsync(
            Guid projectId,
            CancellationToken cancel);

        /// <summary>
        /// Updates the project's default permissions.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="permissions">The new permissions.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        Task<IResult<IImmutableDictionary<string, IPermissions>>> UpdateAllDefaultPermissionsAsync(
            Guid projectId,
            IReadOnlyDictionary<string, IPermissions> permissions,
            CancellationToken cancel);

        /// <summary>
        /// Updates the project after publishing.
        /// </summary>
        /// <param name="projectId">The ID for the project to update.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="newName">The new name of the project, or null to not update the name.</param>
        /// <param name="newDescription">The new description of the project, or null to not update the description.</param>
        /// <param name="newParentProjectId">
        /// The ID of the new parent project, 
        /// or null to not update the parent project,
        /// or <see cref="Guid.Empty"/> to remove the parent project.</param>
        /// <param name="newContentPermissions">The new content permission mode of the project, or null to not update the mode.</param>
        /// <param name="newControllingPermissionsProjectId">
        /// The ID of the new controlling permissions project,
        /// or null to not update the controlling permissions project.
        /// </param>
        /// <param name="newOwnerId">The ID of the new owner of the project, or null to not update the owner.</param>
        /// <returns>The update result.</returns>
        public Task<IResult<IUpdateProjectResult>> UpdateProjectAsync(
            Guid projectId,
            CancellationToken cancel,
            string? newName = null,
            string? newDescription = null,
            Guid? newParentProjectId = null,
            string? newContentPermissions = null,
            Guid? newControllingPermissionsProjectId = null,
            Guid? newOwnerId = null);

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="projectId">The ID for the project to delete.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        public Task<IResult> DeleteProjectAsync(Guid projectId, CancellationToken cancel);
    }
}
