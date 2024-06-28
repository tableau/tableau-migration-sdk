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
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Abstract base class for <see cref="IAuthenticationTypeDomainMapping"/> implementations.
    /// </summary>
    public abstract class AuthenticationTypeDomainMappingBase
        : IAuthenticationTypeDomainMapping
    {
        /// <summary>
        /// Executes the mapping for a user or group.
        /// </summary>
        /// <typeparam name="T">The <see cref="IUsernameContent"/> type.</typeparam>
        /// <param name="context">The mapping context.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The mapped context.</returns>
        protected abstract Task<ContentMappingContext<T>?> ExecuteAsync<T>(ContentMappingContext<T> context, CancellationToken cancel)
            where T : IUsernameContent;

        /// <inheritdoc />
        public async Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
            => await ExecuteAsync<IUser>(ctx, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<ContentMappingContext<IGroup>?> ExecuteAsync(ContentMappingContext<IGroup> ctx, CancellationToken cancel)
            => await ExecuteAsync<IGroup>(ctx, cancel).ConfigureAwait(false);
    }
}
