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

using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Permissions
{
    /// <inheritdoc/>    
    public class Capability : ICapability
    {
        /// <summary>
        /// Creates a new <see cref="Capability"/> instance.
        /// </summary>
        /// <param name="capability">The <see cref="ICapability"/> to create the <see cref="Capability"/> from.</param>
        internal Capability(ICapability capability)
            : this(
                  Guard.AgainstNullEmptyOrWhiteSpace(capability.Name, () => capability.Name),
                  Guard.AgainstNullEmptyOrWhiteSpace(capability.Mode, () => capability.Mode))
        { }

        /// <summary>
        /// Creates a new <see cref="Capability"/> instance.
        /// </summary>
        /// <param name="response">The response to create the <see cref="Capability"/> from.</param>
        internal Capability(CapabilityType response)
            : this(
                  Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name),
                  Guard.AgainstNullEmptyOrWhiteSpace(response.Mode, () => response.Mode))
        { }

        /// <summary>
        /// Creates a new <see cref="Capability"/> instance.
        /// </summary>
        /// <param name="name">The capability name.</param>
        /// <param name="mode">The capability mode from <see cref="PermissionsCapabilityModes"/>.</param>        
        public Capability(string name, string mode)
        {
            Name = Guard.AgainstNullEmptyOrWhiteSpace(name, () => name);
            Mode = Guard.AgainstNullEmptyOrWhiteSpace(mode, () => mode);
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Mode { get; set; }
    }
}
