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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that changes the SiteRole of the user. 
    /// <see cref="SiteRoles.ServerAdministrator"/> on Server changes to <see cref="SiteRoles.SiteAdministratorCreator"/> on the cloud.
    /// See <see href="https://help.tableau.com/current/blueprint/en-us/bp_administrative_roles_responsibilities.htm">Tableau API Reference</see> for details.
    /// </summary>
    public class UserTableauCloudSiteRoleTransformer : ContentTransformerBase<IUser>
    {
        /// <summary>
        /// Creates a new <see cref="UserTableauCloudSiteRoleTransformer"/> object.
        /// </summary>        
        public UserTableauCloudSiteRoleTransformer(
            ISharedResourcesLocalizer localizer,
            ILogger<UserTableauCloudSiteRoleTransformer> logger) 
                : base(localizer, logger)
        { }

        /// <inheritdoc />
        public override Task<IUser?> TransformAsync(IUser itemToTransform, CancellationToken cancel)
        {
            if (string.Equals(itemToTransform.SiteRole, SiteRoles.ServerAdministrator, StringComparison.OrdinalIgnoreCase))
                itemToTransform.SiteRole = SiteRoles.SiteAdministratorCreator;

            return Task.FromResult<IUser?>(itemToTransform);
        }
    }
}
