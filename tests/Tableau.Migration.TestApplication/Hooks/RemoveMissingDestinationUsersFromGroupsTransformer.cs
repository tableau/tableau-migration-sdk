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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Resources;

namespace Tableau.Migration.TestApplication.Hooks
{
    class RemoveMissingDestinationUsersFromGroupsTransformer : ContentTransformerBase<IPublishableGroup>
    {
        private readonly IDestinationContentReferenceFinder<IUser> _destinationUserContentReferenceFinder;
        
        public RemoveMissingDestinationUsersFromGroupsTransformer(
            ISharedResourcesLocalizer localizer,
            ILogger<IContentTransformer<IPublishableGroup>> logger,
            IDestinationContentReferenceFinderFactory destinationContentReferenceFinderFactory)
            : base(localizer, logger)
        {
            _destinationUserContentReferenceFinder = destinationContentReferenceFinderFactory.ForDestinationContentType<IUser>();
        }

        public override async Task<IPublishableGroup?> TransformAsync(
            IPublishableGroup itemToTransform, 
            CancellationToken cancel)
        {
            var updatedUsersList = new List<IGroupUser>();

            foreach (var groupUser in itemToTransform.Users)
            {
                var destinationReference = await _destinationUserContentReferenceFinder.FindByIdAsync(
                    groupUser.User.Id,
                    cancel).
                    ConfigureAwait(false);

                if (destinationReference is not null)
                {
                    updatedUsersList.Add(groupUser);
                }
            }

            itemToTransform.Users = updatedUsersList;

            return itemToTransform;
        }
    }
}
