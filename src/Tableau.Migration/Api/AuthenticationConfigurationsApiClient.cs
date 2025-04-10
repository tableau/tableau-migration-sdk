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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class AuthenticationConfigurationsApiClient :
        ApiClientBase, IAuthenticationConfigurationsApiClient
    {
        /// <summary>
        /// The currently maximum number of allowed authentication configurations.
        /// </summary>
        internal const int MAX_CONFIGURATIONS = 20;

        private readonly IServerSessionProvider _sessionProvider;

        public AuthenticationConfigurationsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IServerSessionProvider sessionProvider,
            ILoggerFactory loggerFactory, 
            ISharedResourcesLocalizer sharedResourcesLocalizer) 
            : base(restRequestBuilderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _sessionProvider = sessionProvider;
        }

        #region - IAuthenticationConfigurationsApiClient Implementation -

        public async Task<IResult<IImmutableList<IAuthenticationConfiguration>>> GetAuthenticationConfigurationsAsync(CancellationToken cancel)
        {
            if(!AssertInstanceType(TableauInstanceType.Cloud, _sessionProvider.InstanceType, throwOnFailure: false))
            {
                return Result<IImmutableList<IAuthenticationConfiguration>>.Succeeded(ImmutableArray<IAuthenticationConfiguration>.Empty);
            }

            var result = await RestRequestBuilderFactory
                .CreateUri("/site-auth-configurations")
                .ForGetRequest()
                .SendAsync<SiteAuthConfigurationsResponse>(cancel)
                .ToResultAsync(r => (IImmutableList<IAuthenticationConfiguration>)r.Items.Select(i => (IAuthenticationConfiguration)new AuthenticationConfiguration(i)).ToImmutableArray(), 
                    SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return result;
        }

        public IPager<IAuthenticationConfiguration> GetPager(int pageSize)
            => new MemoryPager<IAuthenticationConfiguration>(async (c) =>
            {
                var result = await GetAuthenticationConfigurationsAsync(c).ConfigureAwait(false);
                if(!result.Success)
                {
                    return result.CastFailure<IReadOnlyCollection<IAuthenticationConfiguration>>();
                }

                return Result<IReadOnlyCollection<IAuthenticationConfiguration>>.Succeeded(result.Value);
            }, pageSize);

        #endregion
    }
}
