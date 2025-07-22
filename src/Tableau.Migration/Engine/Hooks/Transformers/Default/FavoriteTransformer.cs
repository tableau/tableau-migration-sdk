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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    internal sealed class FavoriteTransformer : ContentTransformerBase<IFavorite>
    {
        private readonly IMappedUserTransformer _userTransformer;
        private readonly IDestinationContentReferenceFinderFactory _destinationFinderFactory;
        private readonly IDestinationViewReferenceFinder _destinationViewFinder;

        public FavoriteTransformer(IMappedUserTransformer userTransformer,
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            IDestinationViewReferenceFinder destinationViewFinder,
            ISharedResourcesLocalizer localizer, ILogger<IContentTransformer<IFavorite>> logger)
            : base(localizer, logger)
        {
            _userTransformer = userTransformer;
            _destinationFinderFactory = destinationFinderFactory;
            _destinationViewFinder = destinationViewFinder;
        }

        private async Task TransformFavoriteUserAsync(IFavorite favorite, CancellationToken cancel)
        {
            var mappedUser = (await _userTransformer.ExecuteAsync(favorite.User, cancel).ConfigureAwait(false))
                .ThrowOnMissingContentReference("Missing destination favorite user reference.");

            favorite.User = mappedUser;
        }

        private async Task TransformFavoriteContentAsync(IFavorite favorite, CancellationToken cancel)
        {
            IContentReference destinationContentReference;
            switch(favorite.ContentType)
            {
                case FavoriteContentType.View:
                    destinationContentReference = (await _destinationViewFinder.FindBySourceIdAsync(favorite.Content.Id, cancel).ConfigureAwait(false))
                        .ThrowOnMissingContentReference("Missing destination favorite view reference.");
                    break;
                default:
                    var contentFinder = _destinationFinderFactory.ForFavoriteDestinationContentType(favorite.ContentType);
                    destinationContentReference = (await contentFinder.FindBySourceLocationAsync(favorite.Content.Location, cancel).ConfigureAwait(false))
                        .ThrowOnMissingContentReference("Missing destination favorite workbook reference.");
                    break;
            }

            favorite.Content = destinationContentReference;
        }

        /// <inheritdoc />
        public override async Task<IFavorite?> TransformAsync(IFavorite itemToTransform, CancellationToken cancel)
        {
            await TransformFavoriteUserAsync(itemToTransform, cancel).ConfigureAwait(false);
            await TransformFavoriteContentAsync(itemToTransform, cancel).ConfigureAwait(false);
            
            return itemToTransform;
        }
    }
}
