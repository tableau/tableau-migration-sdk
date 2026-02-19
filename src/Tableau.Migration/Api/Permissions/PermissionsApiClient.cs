//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Content.Search;
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
        private readonly IContentReferenceFinderFactory _contentReferenceFinderFactory;

        public PermissionsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IHttpContentSerializer serializer,
            IPermissionsUriBuilder uriBuilder,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IContentReferenceFinderFactory contentReferenceFinderFactory)
        {
            _restRequestBuilderFactory = restRequestBuilderFactory;
            _serializer = serializer;
            _uriBuilder = uriBuilder;
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
            _contentReferenceFinderFactory = contentReferenceFinderFactory;
        }

        private async Task<IPermissions> ToPermissionsAsync(PermissionsResponse response, CancellationToken cancel)
        {
            var granteeResponses = response.Item?.GranteeCapabilities;
            
            var grantees = new List<IGranteeCapability>(granteeResponses?.Length ?? 0);
            if(granteeResponses is not null)
            {
                foreach (var granteeResponse in granteeResponses)
                {
                    var finder = _contentReferenceFinderFactory.ForGranteeType(granteeResponse.GranteeType);

                    var granteeRef = (await finder.FindByIdAsync(granteeResponse.GranteeId, cancel).ConfigureAwait(false))
                        .ThrowOnMissingContentReference(_sharedResourcesLocalizer, granteeResponse.GranteeType.ToString(), "grantee", granteeResponse.GranteeId);

                    grantees.Add(new GranteeCapability(granteeRef, granteeResponse));
                }
            }
            
            return new Content.Permissions.Permissions(response.ParentId, grantees);
        }

        public async Task<IResult<IPermissions>> GetPermissionsAsync(Guid id, CancellationToken cancel)
        {
            return await _restRequestBuilderFactory
                .CreatePermissionsUri(_uriBuilder, id)
                .ForGetRequest()
                .SendAsync<PermissionsResponse>(cancel)
                .ToResultAsync(ToPermissionsAsync, _sharedResourcesLocalizer, cancel)
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
                .ToResultAsync(ToPermissionsAsync, _sharedResourcesLocalizer, cancel)
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
