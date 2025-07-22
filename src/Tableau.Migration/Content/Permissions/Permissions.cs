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
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Permissions
{
    internal class Permissions : IPermissions
    {
        /// <inheritdoc/>
        public IList<IGranteeCapability> GranteeCapabilities { get; set; }

        /// <inheritdoc/>
        public Guid? ParentId { get; set; }

        public Permissions(PermissionsResponse response)
            : this(response.ParentId, response.Item?.GranteeCapabilities)
        { }

        public Permissions(PermissionsType permissions)
            : this(permissions.ContentItem?.Id, permissions.GranteeCapabilities)
        { }

        public Permissions(IPermissions permissions)
            : this(permissions.ParentId, permissions.GranteeCapabilities.ToList())
        { }

        public Permissions(Guid? parentId, params IEnumerable<GranteeCapabilityType>? grantees)
            : this(parentId, grantees?.Select(c => (IGranteeCapability)new GranteeCapability(c))?.ToList())
        { }

        public Permissions(Guid? parentId, IList<IGranteeCapability>? granteeCapabilities = null)
        {
            ParentId = parentId;
            GranteeCapabilities = granteeCapabilities ?? new List<IGranteeCapability>();
        }
    }
}