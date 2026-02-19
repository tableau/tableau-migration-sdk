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
    /// Transformer that transforms the list of users that have the custom view as default.
    /// It sets the references of these users to those at the destination.
    /// </summary>
    public class CustomViewDefaultUserReferencesTransformer
        : ContentTransformerBase<IPublishableCustomView>
    {
        private readonly IDestinationContentReferenceFinder<IUser> _userFinder;

        /// <summary>
        /// Creates a new <see cref="CustomViewDefaultUserReferencesTransformer"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        public CustomViewDefaultUserReferencesTransformer(IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ISharedResourcesLocalizer localizer, ILogger<CustomViewDefaultUserReferencesTransformer> logger)
            : base(localizer, logger)
        {
            _userFinder = destinationFinderFactory.ForDestinationContentType<IUser>();
        }

        /// <inheritdoc />
        public override async Task<IPublishableCustomView?> TransformAsync(
            IPublishableCustomView sourceCustomView,
            CancellationToken cancel)
        {
            var missingUsers = new List<ContentLocation>();

            for (var i = 0; i < sourceCustomView.DefaultUsers.Count; i++)
            {
                var destinationUser = await _userFinder.FindBySourceLocationAsync(sourceCustomView.DefaultUsers[i].Location, cancel)
                    .ConfigureAwait(false);

                if (destinationUser is null)
                {
                    missingUsers.Add(sourceCustomView.DefaultUsers[i].Location);
                    continue;
                }

                sourceCustomView.DefaultUsers[i] = destinationUser;
            }

            missingUsers.ThrowOnMissingContentReferences<IUser>(Localizer, "custom view default users");

            return sourceCustomView;
        }
    }
}
