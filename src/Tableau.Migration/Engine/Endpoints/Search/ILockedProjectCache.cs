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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Interface for an object that contains information on projects that are locked.
    /// </summary>
    public interface ILockedProjectCache
    {
        /// <summary>
        /// Finds whether a project is locked.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="includeWithoutNested">
        /// Whether or not to consider <see cref="ContentPermissions.LockedToProjectWithoutNested"/> as locked.
        /// Except for narrow special cases this is true.
        /// </param>
        /// <returns>True if the project is locked; false if the project is not locked or not found.</returns>
        Task<bool> IsProjectLockedAsync(Guid id, CancellationToken cancel, bool includeWithoutNested = true);

        /// <summary>
        /// Updates the locked project cache with the given project information.
        /// </summary>
        /// <param name="project">The project to update the cache for.</param>
        void UpdateLockedProjectCache(IProject project);
    }
}
