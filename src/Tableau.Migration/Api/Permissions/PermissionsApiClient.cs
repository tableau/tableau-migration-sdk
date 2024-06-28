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
using System.Collections.Immutable;
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

        internal virtual ImmutableArray<IGranteeCapability> GetCapabilitiesToDelete(
            IGranteeCapability[] sourceItems,
            IGranteeCapability[] destinationItems)
        {
            return destinationItems.Except(sourceItems).ToImmutableArray();
        }

        public async virtual Task<IResult<IPermissions>> GetPermissionsAsync(
            Guid id,
            CancellationToken cancel)
        {
            return await _restRequestBuilderFactory
                .CreatePermissionsUri(_uriBuilder, id)
                .ForGetRequest()
                .SendAsync<PermissionsResponse>(cancel)
                .ToResultAsync<PermissionsResponse, IPermissions>(p => new Content.Permissions.Permissions(p), _sharedResourcesLocalizer)
                .ConfigureAwait(false);
        }

        public async virtual Task<IResult<IPermissions>> CreatePermissionsAsync(
            Guid contentItemId,
            IPermissions permissions,
            CancellationToken cancel)
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

        public async virtual Task<IResult> DeleteCapabilityAsync(
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

        public async Task<IResult> DeleteAllPermissionsAsync(
            Guid contentItemId,
            IPermissions destinationPermissions,
            CancellationToken cancel)
        {
            return await DeletePermissionsAsync(
                contentItemId,
                new Content.Permissions.Permissions(),
                destinationPermissions,
                cancel)
                .ConfigureAwait(false);
        }

        public async virtual Task<IResult> DeletePermissionsAsync(
            Guid contentItemId,
            IPermissions sourcePermissions,
            IPermissions destinationPermissions,
            CancellationToken cancel)
        {
            var itemsToDelete = GetCapabilitiesToDelete(
                sourcePermissions.GranteeCapabilities,
                destinationPermissions.GranteeCapabilities);

            if (itemsToDelete.IsNullOrEmpty())
            {
                return Result.Succeeded();
            }

            var resultBuilder = new ResultBuilder();

            foreach (var item in itemsToDelete)
            {
                var granteeId = item.GranteeId;

                foreach (var capability in item.Capabilities)
                {
                    var deleteResult = await DeleteCapabilityAsync(
                        contentItemId,
                        granteeId,
                        item.GranteeType,
                        capability,
                        cancel)
                        .ConfigureAwait(false);

                    if (!deleteResult.Success)
                    {
                        resultBuilder.Add(deleteResult);
                    }
                }
            }

            var result = resultBuilder.Build();

            if (!result.Success)
            {
                return Result.Failed(result.Errors);
            }
            return Result.Succeeded();
        }

        public async virtual Task<IResult<IPermissions>> UpdatePermissionsAsync(
            Guid contentItemId,
            IPermissions sourcePermissions,
            CancellationToken cancel)
        {
            var destinationPermissionsResult = await GetPermissionsAsync(
                 contentItemId,
                 cancel)
                 .ConfigureAwait(false);

            if (!destinationPermissionsResult.Success)
            {
                return Result<IPermissions>.Failed(destinationPermissionsResult.Errors);
            }

            var deleteResult = await DeletePermissionsAsync(
                contentItemId,
                sourcePermissions,
                destinationPermissionsResult.Value,
                cancel)
                .ConfigureAwait(false);

            if (!deleteResult.Success)
            {
                return Result<IPermissions>.Failed(deleteResult.Errors);
            }

            return await CreatePermissionsAsync(
                contentItemId,
                sourcePermissions,
                cancel)
                .ConfigureAwait(false);
        }
    }
}
