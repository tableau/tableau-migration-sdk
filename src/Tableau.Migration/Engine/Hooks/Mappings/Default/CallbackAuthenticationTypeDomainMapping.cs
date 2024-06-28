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
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// <see cref="AuthenticationTypeDomainMapping"/> implementation that wraps a callback function.
    /// </summary>
    public class CallbackAuthenticationTypeDomainMapping
        : AuthenticationTypeDomainMappingBase
    {
        private readonly Func<ContentMappingContext<IUsernameContent>, CancellationToken, Task<string?>> _callback;

        /// <summary>
        /// Creates a new <see cref="CallbackAuthenticationTypeDomainMapping"/> object.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        public CallbackAuthenticationTypeDomainMapping(Func<ContentMappingContext<IUsernameContent>, CancellationToken, Task<string?>> callback)
        {
            _callback = callback;
        }

        /// <summary>
        /// Executes the mapping for a user or group.
        /// </summary>
        /// <typeparam name="T">The <see cref="IUsernameContent"/> type.</typeparam>
        /// <param name="context">The mapping context.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The mapped context.</returns>
        protected override async Task<ContentMappingContext<T>?> ExecuteAsync<T>(ContentMappingContext<T> context, CancellationToken cancel)
        {
            var interfaceContext = new ContentMappingContext<IUsernameContent>(context.ContentItem, context.MappedLocation);
            var newDomain = await _callback(interfaceContext, cancel).ConfigureAwait(false);
            if (newDomain is null)
            {
                return context;
            }
            else
            {
                return context.MapTo(ContentLocation.ForUsername(newDomain, context.MappedLocation.Name));
            }
        }
    }
}
