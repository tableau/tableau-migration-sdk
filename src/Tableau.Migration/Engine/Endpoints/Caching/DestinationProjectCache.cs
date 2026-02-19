//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Endpoints.Caching
{
    /// <summary>
    /// <see cref="DestinationCache{IProject}"/> implementation that tracks locked projects.
    /// </summary>
    public class DestinationProjectCache : DestinationCache<IProject>, ILockedProjectCache
    {
        private readonly ConcurrentDictionary<Guid, string> _projectContentPermissionModeCache;

        /// <summary>
        /// Creates a new <see cref="DestinationProjectCache"/> object.
        /// </summary>
        /// <param name="pipeline"><inheritdoc /></param>
        /// <param name="endpoint">The destination endpoint.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="manifest">The migration manifest.</param>
        /// <param name="logger"><inheritdoc /></param>
        public DestinationProjectCache(
            IMigrationPipeline pipeline,
            IDestinationEndpoint endpoint,
            IConfigReader configReader,
            IMigrationManifestEditor manifest,
            ILogger<DestinationProjectCache> logger)
            : base(pipeline, endpoint, configReader, manifest, logger)
        {
            _projectContentPermissionModeCache = new();
        }

        /// <inheritdoc />
        protected override async Task ItemsLoadedAsync(IImmutableList<IProject> items, CancellationToken cancel)
        {
            await base.ItemsLoadedAsync(items, cancel).ConfigureAwait(false);

            foreach(var item in items)
            {
                UpdateLockedProjectCache(item);
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsProjectLockedAsync(Guid id, CancellationToken cancel, bool includeWithoutNested = true)
        {
            await ForIdAsync(id, cancel).ConfigureAwait(false);

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
