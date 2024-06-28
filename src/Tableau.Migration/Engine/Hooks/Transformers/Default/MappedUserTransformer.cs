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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the user for a content item owner.
    /// </summary>
    public class MappedUserTransformer : IMappedUserTransformer
    {
        private readonly IDestinationContentReferenceFinder<IUser> _userFinder;
        private readonly ILogger _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="MappedUserTransformer"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="logger">The logger used to log messages.</param>
        /// <param name="localizer">The string localizer.</param>
        public MappedUserTransformer(
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ILogger<MappedUserTransformer> logger,
            ISharedResourcesLocalizer localizer)
        {
            _userFinder = destinationFinderFactory.ForDestinationContentType<IUser>();
            _logger = logger;
            _localizer = localizer;
        }

        /// <inheritdoc />
        public async Task<IContentReference?> ExecuteAsync(IContentReference ctx, CancellationToken cancel)
        {
            //Unable to map system user, as its info is hidden from APIs except for owner references.
            if(ctx.Location == Constants.SystemUserLocation)
            {
                return null;
            }

            var mapped = await _userFinder.FindBySourceLocationAsync(ctx.Location, cancel)
                .ConfigureAwait(false);

            if (mapped is null)
            {
                _logger.LogWarning(_localizer[SharedResourceKeys.SourceUserNotFoundLogMessage], ctx.Name, ctx.Id);
            }

            return mapped;
        }
    }
}
