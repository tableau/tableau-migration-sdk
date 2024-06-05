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

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// The license levels to be derived from a siterole in <see cref="SiteRoles"/>.
    /// </summary>
    public class LicenseLevels : StringEnum<LicenseLevels>
    {
        /// <summary>
        /// Gets the name of the Creator license level.
        /// </summary>
        public const string Creator = "Creator";

        /// <summary>
        /// Gets the name of the Explorer license level.
        /// </summary>
        public const string Explorer = "Explorer";

        /// <summary>
        /// Gets the name of the Viewer license level.
        /// </summary>
        public const string Viewer = "Viewer";

        /// <summary>
        /// Gets the name of the license level when a user is unlicensed.
        /// </summary>
        public const string Unlicensed = "Unlicensed";
    }
}
