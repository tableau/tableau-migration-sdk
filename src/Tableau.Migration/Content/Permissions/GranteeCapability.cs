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

using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Permissions
{
    /// <inheritdoc/>
    public class GranteeCapability : IGranteeCapability
    {
        internal GranteeCapability(IContentReference grantee, GranteeCapabilityType response)
            : this(response.GranteeType, grantee, response.Capabilities.Select(c => new Capability(c)))
        { }

        internal GranteeCapability(GranteeType granteeType, IContentReference grantee, IEnumerable<ICapability> capabilities)
        {
            GranteeType = granteeType;
            Grantee = grantee;

            Capabilities = new HashSet<ICapability>(capabilities, ICapabilityComparer.Instance);
        }

        /// <inheritdoc/>
        public virtual void ResolveCapabilityModeConflicts()
            => Capabilities.ResolveCapabilityModeConflicts();

        /// <inheritdoc/>
        public GranteeType GranteeType { get; set; }

        /// <inheritdoc/>
        public IContentReference Grantee { get; set; }

        /// <inheritdoc/>
        public HashSet<ICapability> Capabilities { get; set; }
    }
}
