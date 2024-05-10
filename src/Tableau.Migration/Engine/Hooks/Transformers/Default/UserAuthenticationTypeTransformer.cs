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
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Options;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that provides an authentication type for users, 
    /// defaulting to <see cref="AuthenticationTypes.ServerDefault"/>.
    /// See <see href="https://help.tableau.com/current/blueprint/en-gb/bp_administrative_roles_responsibilities.htm">Tableau API Reference</see> for details.
    /// </summary>
    public class UserAuthenticationTypeTransformer : ContentTransformerBase<IUser>
    {
        private readonly string _authenticationType;

        /// <summary>
        /// Creates a new <see cref="UserAuthenticationTypeTransformer"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options provider.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">The default logger.</param>
        public UserAuthenticationTypeTransformer(
            IMigrationPlanOptionsProvider<UserAuthenticationTypeTransformerOptions> optionsProvider,
            ISharedResourcesLocalizer localizer,
            ILogger<UserAuthenticationTypeTransformer> logger) 
                : base(localizer, logger)
        {
            _authenticationType = optionsProvider.Get().AuthenticationType;
        }

        /// <inheritdoc />
        public override Task<IUser?> TransformAsync(IUser itemToTransform, CancellationToken cancel)
        {
            itemToTransform.AuthenticationType = _authenticationType;
            return Task.FromResult<IUser?>(itemToTransform);
        }
    }
}
