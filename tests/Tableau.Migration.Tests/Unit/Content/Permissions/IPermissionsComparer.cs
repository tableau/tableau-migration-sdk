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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Tests.Content.Permissions
{
    internal class IPermissionsComparer : ComparerBase<IPermissions>
    {
        public static readonly IPermissionsComparer Instance = new();

        protected override int CompareItems(IPermissions x, IPermissions y)
        {
            var parentIdResult = Comparer<Guid?>.Default.Compare(x.ParentId, y.ParentId);

            if (parentIdResult != 0)
                return parentIdResult;

            return IGranteeCapabilityComparer.Instance.Compare(x.GranteeCapabilities, y.GranteeCapabilities);
        }

        public bool Equals(PermissionsType x, IPermissions y) => Equals(new Migration.Content.Permissions.Permissions(x), y);

        public override int GetHashCode([DisallowNull] IPermissions obj)
            => HashCode.Combine(
                obj.ParentId,
                IGranteeCapabilityComparer.Instance.GetHashCode(obj.GranteeCapabilities));
    }
}
