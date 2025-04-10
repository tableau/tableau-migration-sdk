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

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that provides an authentication type for users, 
    /// defaulting to <see cref="AuthenticationTypes.ServerDefault"/>.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#add_user_to_site">Tableau API Reference</see> for details.
    /// </summary>
    public class UserAuthenticationTypeTransformer : ContentTransformerBase<IUser>
    {
        private readonly IDestinationAuthenticationConfigurationsCache _destinationAuthConfigCache;
        private readonly string _authenticationType;

        /// <summary>
        /// Creates a new <see cref="UserAuthenticationTypeTransformer"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options provider.</param>
        /// <param name="destinationAuthConfigCache">The destination authentication configuration cache.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">The default logger.</param>
        public UserAuthenticationTypeTransformer(
            IMigrationPlanOptionsProvider<UserAuthenticationTypeTransformerOptions> optionsProvider,
            IDestinationAuthenticationConfigurationsCache destinationAuthConfigCache,
            ISharedResourcesLocalizer localizer,
            ILogger<UserAuthenticationTypeTransformer> logger) 
                : base(localizer, logger)
        {
            _destinationAuthConfigCache = destinationAuthConfigCache;
            _authenticationType = optionsProvider.Get().AuthenticationType;
        }

        /// <inheritdoc />
        public override async Task<IUser?> TransformAsync(IUser itemToTransform, CancellationToken cancel)
        {
            var configs = await _destinationAuthConfigCache.GetAllAsync(cancel).ConfigureAwait(false);
            if(!configs.Any())
            {
                /* 
                 * If the site has no authentication configurations it does not support multiple authentication types.
                 * We directly set the auth type moniker in this case.
                 */
                itemToTransform.Authentication = UserAuthenticationType.ForAuthenticationType(_authenticationType);
                return itemToTransform;
            }

            /*
             * If the site has multiple authentication configurations it supports multiple authentication types.
             * We need to determine the IdP configuration ID to assign to the user.
             * We match based on the UI display name first, but fall back to matching on the authSetting
             * if the migration user relied on a plan build method and forgot to give us the IdP name.
             */
            var configsByName = configs
                .Where(c => string.Equals(_authenticationType, c.IdpConfigurationName, ContentBase.NameComparison))
                .ToImmutableArray();

            if(configsByName.Length == 1)
            {
                itemToTransform.Authentication = UserAuthenticationType.ForConfigurationId(configsByName.Single().Id);
                return itemToTransform;
            }

            var configByAuthSetting = configs
                .Where(c => string.Equals(_authenticationType, c.AuthSetting, ContentBase.NameComparison))
                .ToImmutableArray();

            if(configByAuthSetting.Length == 1)
            {
                itemToTransform.Authentication = UserAuthenticationType.ForConfigurationId(configByAuthSetting.Single().Id);
                return itemToTransform;
            }

            if(configsByName.Length > 1 || configByAuthSetting.Length > 1)
            {
                throw new ArgumentException($"Multiple authentication configurations were found with name or auth setting \"{_authenticationType}\".");
            }

            throw new ArgumentException($"No authentication configurations were found with name or auth setting \"{_authenticationType}\".");
        }
    }
}
