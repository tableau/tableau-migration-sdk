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
using System.Diagnostics.CodeAnalysis;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Tests.Content.Permissions
{
    internal class IGranteeCapabilityComparer : ComparerBase<IGranteeCapability>
    {
        public static IGranteeCapabilityComparer Instance = new();

        private readonly bool _compareGranteeIds;

        public IGranteeCapabilityComparer(bool compareGranteeIds = true) => _compareGranteeIds = compareGranteeIds;

        protected override int CompareItems(IGranteeCapability x, IGranteeCapability y)
        {
            var granteeIdResult = _compareGranteeIds ? x.GranteeId.CompareTo(y.GranteeId) : 0;

            if (granteeIdResult != 0)
                return granteeIdResult;

            var granteeTypeResult = x.GranteeType.CompareTo(y.GranteeType);

            if (granteeTypeResult != 0)
                return granteeTypeResult;

            return ICapabilityComparer.Instance.Compare(x.Capabilities, y.Capabilities);
        }

        public override int GetHashCode([DisallowNull] IGranteeCapability obj)
            => HashCode.Combine(_compareGranteeIds ? obj.GranteeId : Guid.Empty, obj.GranteeType, ICapabilityComparer.Instance.GetHashCode(obj.Capabilities));
    }
}
