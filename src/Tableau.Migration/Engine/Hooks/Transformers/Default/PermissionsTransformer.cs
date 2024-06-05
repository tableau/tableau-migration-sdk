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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the users or groups from a given permission.
    /// </summary>
    public class PermissionsTransformer : IPermissionsTransformer
    {
        private readonly IDestinationContentReferenceFinder<IUser> _userContentFinder;
        private readonly IDestinationContentReferenceFinder<IGroup> _groupContentFinder;
        private readonly ILogger<PermissionsTransformer> _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="PermissionsTransformer"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="logger">Default logger.</param>
        /// <param name="localizer">A string localizer.</param>
        public PermissionsTransformer(
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ILogger<PermissionsTransformer> logger, 
            ISharedResourcesLocalizer localizer)
        {
            _userContentFinder = destinationFinderFactory.ForDestinationContentType<IUser>();
            _groupContentFinder = destinationFinderFactory.ForDestinationContentType<IGroup>();
            _logger = logger;
            _localizer = localizer;
        }

        private static bool ShouldMigrateCapability(ICapability c)
        {
            //W-14374726 Some versions of Tableau Server (pre-2020.1) supported ProjectLeader Deny capabilities,
            //but that feature was removed. These capabilities remain in the database (even after upgrade)
            //but will throw errors when migrated through APIs.
            //Thus we filter out these capabilities to avoid errors the user has no control over.
            if (PermissionsCapabilityNames.IsAMatch(PermissionsCapabilityNames.ProjectLeader, c.Name))
            {
                if (PermissionsCapabilityModes.IsAMatch(PermissionsCapabilityModes.Deny, c.Mode))
                {
                    return false;
                }
            }

            //Inherited leaders are calculated automatically and don't need to be set manually.
            if (PermissionsCapabilityNames.IsAMatch(PermissionsCapabilityNames.InheritedProjectLeader, c.Name))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<IImmutableList<IGranteeCapability>?> ExecuteAsync(
            IImmutableList<IGranteeCapability> granteeCapabilities,
            CancellationToken cancel)
        {
            var transformedGrantees = new List<IGranteeCapability>();

            var groupsById = new HashSet<IGranteeCapability>(granteeCapabilities).GroupBy(c => c.GranteeId);

            foreach (var group in groupsById)
            {
                var granteeType = group.First().GranteeType;

                var destinationGrantee = await GetDestinationGranteeAsync(
                    group.Key,
                    granteeType, 
                    cancel)
                    .ConfigureAwait(false);

                if (destinationGrantee is null)
                {
                    _logger.LogWarning(_localizer.GetString(SharedResourceKeys.PermissionsTransformerGranteeNotFoundWarning), granteeType.ToString(), group.Key);
                    continue;
                }

                var destinationCapabilities = group
                    .SelectMany(g => g.Capabilities)
                    .Where(ShouldMigrateCapability)
                    .ResolveCapabilityModeConflicts();

                //Capability resolution automatically happens here since this
                //GranteeCapability constructor applies that logic.
                var transformedGrantee = new GranteeCapability(
                    granteeType,
                    destinationGrantee.Id,
                    destinationCapabilities);

                transformedGrantees.Add(transformedGrantee);
            }

            return transformedGrantees.ToImmutableArray();
        }

        private async Task<IContentReference?> GetDestinationGranteeAsync(
            Guid groupKey, 
            GranteeType granteeType, 
            CancellationToken cancel)
        {
            if (granteeType is GranteeType.User)
            {
                return await _userContentFinder
                    .FindBySourceIdAsync(groupKey, cancel)
                    .ConfigureAwait(false);
            }

            return await _groupContentFinder
                .FindBySourceIdAsync(groupKey, cancel)
                .ConfigureAwait(false);
        }
    }
}
