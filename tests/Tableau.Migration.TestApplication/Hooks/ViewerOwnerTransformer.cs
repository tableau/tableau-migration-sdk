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
using Microsoft.Extensions.Options;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Resources;
using Tableau.Migration.TestApplication.Config;


namespace Tableau.Migration.TestApplication.Hooks
{
    /// <summary>
    /// There is a lot of content on Kibble that is owned by users that are licensed as "Viewer".
    /// This can only happen if a user was a "Creator" or above and then was downgraded to "Viewer".
    /// When a migration happens, we need to ensure that the owner of the content is not a "Viewer" as they can't be owners.
    /// </summary>
    public class ViewerOwnerTransformer<TPublish> : ContentTransformerBase<TPublish>
        where TPublish : IContentReference, IWithOwner
    {
        private readonly ContentLocation _adminUser;
        private readonly IDestinationContentReferenceFinder<IUser> _destinationContentReferenceFinder;
        private readonly IUsersApiClient _usersApiClient;

        public ViewerOwnerTransformer(
            ISharedResourcesLocalizer localizer,
            ILogger<ViewerOwnerTransformer<TPublish>> logger,
            IDestinationContentReferenceFinder<IUser> destinationContentReferenceFinder,
            IMigration migration,
            IOptions<TestApplicationOptions> options)
            : base(localizer, logger)
        {
            var _options = options.Value;

            _adminUser = ContentLocation.ForUsername(_options.SpecialUsers.AdminDomain, _options.SpecialUsers.AdminUsername);
            _destinationContentReferenceFinder = destinationContentReferenceFinder;
            _usersApiClient = ((TableauApiDestinationEndpoint)migration.Destination).SiteApi.Users;
        }

        public async override Task<TPublish?> TransformAsync(TPublish itemToTransform, CancellationToken cancel)
        {
            // At this point, the owner of the itemToTransform is already mapped, so we need to find it on the destination
            var owner = await _destinationContentReferenceFinder.FindByMappedLocationAsync(itemToTransform.Owner.Location, cancel) as IUser;

            if (owner == null)
            {
                // If owner is null, then the _destinationContentReferenceFinder either couldn't find the user, or it was a ContentReferenceStub that couldn't be turned into an IUser
                // A ContentReferenceStub doesn't have the license level, so we need to get the owner from the API to get the complete content item.
                var ownerResult = await _usersApiClient.GetByIdAsync(itemToTransform.Owner.Id, cancel).ConfigureAwait(false);
                if (ownerResult.Success)
                {
                    owner = ownerResult.Value;
                }
            }

            if (owner == null)
            {
                throw new System.Exception($"Owner {itemToTransform.Owner.Location} for {typeof(TPublish).GetFormattedName()} named {itemToTransform.Location} could not be found.");
            }

            if (owner.LicenseLevel == LicenseLevels.Viewer)
            {
                var adminUser = await _destinationContentReferenceFinder.FindByMappedLocationAsync(_adminUser, cancel);

                if (adminUser == null)
                {
                    throw new System.Exception($"Admin user {_adminUser}  not found");
                }

                itemToTransform.Owner = adminUser;

            }

            return itemToTransform;
        }
    }
}
