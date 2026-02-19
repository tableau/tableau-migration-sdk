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
    /// Transformer that maps the user for a content item owner.
    /// </summary>
    public class OwnershipTransformer<TContent> : ContentTransformerBase<TContent>
        where TContent : IWithOwner
    {
        private readonly IDestinationContentReferenceFinder<IUser> _userFinder;        

        /// <summary>
        /// Creates a new <see cref="OwnershipTransformer{TContent}"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        public OwnershipTransformer(IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ISharedResourcesLocalizer localizer, ILogger<OwnershipTransformer<TContent>> logger)
            : base(localizer, logger)
        {
            _userFinder = destinationFinderFactory.ForDestinationContentType<IUser>();
        }

        /// <inheritdoc/>
        public override async Task<TContent?> TransformAsync(TContent itemToTransform, CancellationToken cancel)
        {
            /*
             * Unable to map system user, as its info is hidden from APIs except for owner references.
             * Our post-publish counts system user ownership as a no-op, so we don't transform it here.
             * If our post-publish logic changes to support system user location mapping we should update it here.
             */
            if (itemToTransform.Owner.Location == Constants.SystemUserLocation)
            {
                return itemToTransform;
            }

            var mappedOwner = (await _userFinder.FindBySourceLocationAsync(itemToTransform.Owner.Location, cancel).ConfigureAwait(false))
                .ThrowOnMissingContentReference<IUser>(Localizer, "owner", itemToTransform.Owner.Location);

            itemToTransform.Owner = mappedOwner;
            return itemToTransform;
        }
    }
}
