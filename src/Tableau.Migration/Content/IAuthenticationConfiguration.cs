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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for an authentication configuration
    /// </summary>
    public interface IAuthenticationConfiguration : IContentReference
    {
        /// <summary>
        /// Gets the auth setting name.
        /// </summary>
        public string AuthSetting { get; }

        /// <summary>
        /// Gets the known provider alias.
        /// </summary>
        public string? KnownProviderAlias { get; }

        /// <summary>
        /// Gets the IdP configuration name.
        /// </summary>
        public string IdpConfigurationName { get; }

        /// <summary>
        /// Gets the enabled flag.
        /// </summary>
        public bool Enabled { get; }
    }
}
