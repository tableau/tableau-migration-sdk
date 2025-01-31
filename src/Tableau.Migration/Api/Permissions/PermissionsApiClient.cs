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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Permissions
{
    internal class PermissionsApiClient : IPermissionsApiClient
    {
        private readonly IRestRequestBuilderFactory _restRequestBuilderFactory;
        private readonly IHttpContentSerializer _serializer;
        private readonly IPermissionsUriBuilder _uriBuilder;
        private readonly ISharedResourcesLocalizer _sharedResourcesLocalizer;

        public PermissionsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IHttpContentSerializer serializer,
            IPermissionsUriBuilder uriBuilder,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
        {
            _restRequestBuilderFactory = restRequestBuilderFactory;
            _serializer = serializer;
            _uriBuilder = uriBuilder;
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
        }

        public async Task<IResult<IPermissions>> GetPermissionsAsync(Guid id, CancellationToken cancel)
        {
            return await _restRequestBuilderFactory
                .CreatePermissionsUri(_uriBuilder, id)
                .ForGetRequest()
                .SendAsync<PermissionsResponse>(cancel)
                .ToResultAsync<PermissionsResponse, IPermissions>(p => new Content.Permissions.Permissions(p), _sharedResourcesLocalizer)
                .ConfigureAwait(false);
        }

        public async Task<IResult<IPermissions>> CreatePermissionsAsync(Guid contentItemId, IPermissions permissions, CancellationToken cancel)
        {
            // The REST API will error if you try to update with no grantees. 
            if (!permissions.GranteeCapabilities.Any())
                return Result<IPermissions>.Succeeded(permissions);

            return await _restRequestBuilderFactory
                .CreatePermissionsUri(_uriBuilder, contentItemId)
                .ForPutRequest()
                .WithXmlContent(new PermissionsAddRequest(permissions))
                .SendAsync<PermissionsResponse>(cancel)
                .ToResultAsync<PermissionsResponse, IPermissions>(p => new Content.Permissions.Permissions(p), _sharedResourcesLocalizer)
                .ConfigureAwait(false);
        }

        public async Task<IResult> DeleteCapabilityAsync(
            Guid contentItemId,
            Guid granteeId,
            GranteeType granteeType,
            ICapability capability,
            CancellationToken cancel)
        {
            return await _restRequestBuilderFactory
                .CreatePermissionsDeleteUri(_uriBuilder, contentItemId, capability, granteeType, granteeId)
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, _sharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }

        public async Task<IResult> UpdatePermissionsAsync(Guid contentItemId, IPermissions permissions, CancellationToken cancel)
        {
            return await _restRequestBuilderFactory
                .CreatePermissionsUri(_uriBuilder, contentItemId)
                .ForPostRequest()
                .WithXmlContent(new PermissionsAddRequest(permissions))
                .SendAsync(cancel)
                .ToResultAsync(_serializer, _sharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }
    }
}
