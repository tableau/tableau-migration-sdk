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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
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

        public ViewerOwnerTransformer(
            ISharedResourcesLocalizer localizer,
            ILogger<IContentTransformer<TPublish>> logger,
            IDestinationContentReferenceFinder<IUser> destinationContentReferenceFinder,
            IOptions<TestApplicationOptions> options)
            : base(localizer, logger)
        {
            var _options = options.Value;

            _adminUser = ContentLocation.ForUsername(_options.SpecialUsers.AdminDomain, _options.SpecialUsers.AdminUsername);
            _destinationContentReferenceFinder = destinationContentReferenceFinder;
        }

        public async override Task<TPublish?> TransformAsync(TPublish itemToTransform, CancellationToken cancel)
        {
            var owner = await _destinationContentReferenceFinder.FindByIdAsync(itemToTransform.Owner.Id, cancel) as IUser;

            if (owner == null)
            {
                throw new System.Exception("Owner not found");
            }

            if (owner.LicenseLevel == LicenseLevels.Viewer)
            {
                var adminUser = await _destinationContentReferenceFinder.FindBySourceLocationAsync(_adminUser, cancel);

                if (adminUser == null)
                {
                    throw new System.Exception("Admin user not found");
                }

                itemToTransform.Owner = adminUser;

            }

            return itemToTransform;
        }
    }
}
