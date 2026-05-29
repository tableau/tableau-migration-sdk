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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the groups from a given group set.
    /// </summary>
    public class GroupSetGroupsTransformer : ContentTransformerBase<IPublishableGroupSet>
    {
        private readonly IDestinationContentReferenceFinder<IGroup> _groupFinder;

        /// <summary>
        /// Creates a new <see cref="GroupSetGroupsTransformer"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        public GroupSetGroupsTransformer(IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ISharedResourcesLocalizer localizer, ILogger<GroupSetGroupsTransformer> logger)
            : base(localizer, logger)
        {
            _groupFinder = destinationFinderFactory.ForDestinationContentType<IGroup>();
        }

        /// <inheritdoc />
        public override async Task<IPublishableGroupSet?> TransformAsync(IPublishableGroupSet sourceGroupSet, CancellationToken cancel)
        {
            var transformedGroups = new List<IContentReference>(sourceGroupSet.Groups.Count);
            var missingGroups = new List<ContentLocation>();

            foreach (var group in sourceGroupSet.Groups)
            {
                var destinationGroup = await _groupFinder.FindResultBySourceLocationAsync(group.Location, cancel)
                    .ConfigureAwait(false);

                if(destinationGroup.Status is Manifest.MigrationManifestEntryStatus.Skipped)
                {
                    continue;
                }

                if (destinationGroup.Destination is null)
                {
                    missingGroups.Add(group.Location);
                    continue;
                }

                transformedGroups.Add(destinationGroup.Destination);
            }

            missingGroups.ThrowOnMissingContentReferences<IGroup>(Localizer, "group set groups");

            sourceGroupSet.Groups = transformedGroups;
            return sourceGroupSet;
        }
    }
}
