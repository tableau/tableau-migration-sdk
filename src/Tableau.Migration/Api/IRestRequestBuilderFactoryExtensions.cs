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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api
{
    internal static class IRestRequestBuilderFactoryExtensions
    {
        public static IRestRequestBuilder CreatePermissionsUri(
            this IRestRequestBuilderFactory requestBuilderFactory,
            IPermissionsUriBuilder permissionsUriBuilder,
            Guid contentItemId)
        {
            return requestBuilderFactory.CreateUri(permissionsUriBuilder.BuildUri(contentItemId));
        }

        public static IRestRequestBuilder CreatePermissionsDeleteUri(
            this IRestRequestBuilderFactory requestBuilderFactory,
            IPermissionsUriBuilder permissionsUriBuilder,
            Guid contentItemId,
            ICapability capability,
            GranteeType granteeType,
            Guid granteeId)
        {
            return requestBuilderFactory.CreateUri(permissionsUriBuilder.BuildDeleteUri(contentItemId, capability, granteeType, granteeId));
        }
    }
}
