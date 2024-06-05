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

namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// The interface for a grantee's capability.
    /// </summary>
    public interface ICapability
    {
        /// <summary>
        /// The capability name from <see cref="PermissionsCapabilityNames"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The capability mode from <see cref="PermissionsCapabilityModes"/>.
        /// </summary>
        public string Mode { get; }
    }
}