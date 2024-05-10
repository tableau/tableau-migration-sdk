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
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="BulkDestinationCache{IProject}"/> implementation that tracks locked projects.
    /// </summary>
    public class BulkDestinationProjectCache : BulkDestinationCache<IProject>, ILockedProjectCache
    {
        private readonly ConcurrentDictionary<Guid, string> _projectContentPermissionModeCache;

        /// <summary>
        /// Creates a new <see cref="BulkDestinationProjectCache"/> object.
        /// </summary>
        /// <param name="endpoint">The destination endpoint.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="manifest">The migration manifest.</param>
        public BulkDestinationProjectCache(
            IDestinationEndpoint endpoint, 
            IConfigReader configReader, 
            IMigrationManifestEditor manifest) 
            : base(endpoint, configReader, manifest)
        {
            _projectContentPermissionModeCache = new();
        }

        /// <inheritdoc />
        protected override void ItemLoaded(IProject item)
        {
            base.ItemLoaded(item);
            UpdateLockedProjectCache(item);
        }

        /// <inheritdoc />
        public async Task<bool> IsProjectLockedAsync(Guid id, CancellationToken cancel, bool includeWithoutNested = true)
        {
            await SearchAsync(id, cancel).ConfigureAwait(false);

            if (!_projectContentPermissionModeCache.TryGetValue(id, out var mode))
            {
                return false;
            }

            if (ContentPermissions.IsAMatch(ContentPermissions.LockedToProject, mode))
            {
                return true;
            }
            else if (includeWithoutNested && ContentPermissions.IsAMatch(ContentPermissions.LockedToProjectWithoutNested, mode))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void UpdateLockedProjectCache(IProject project)
        {
            _projectContentPermissionModeCache.AddOrUpdate(project.Id, project.ContentPermissions, (k, v) => project.ContentPermissions);
        }
    }
}
