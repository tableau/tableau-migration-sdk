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
    /// Class containing extract encryption mode constants.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_extract_and_encryption.htm">Tableau API Reference</see> 
    /// for documentation.
    /// </summary>
    public class ExtractEncryptionModes : StringEnum<ExtractEncryptionModes>
    {
        /// <summary>
        /// The mode to enforce encryption of all extracts on the site. 
        /// </summary>
        public const string Enforced = "enforced";

        /// <summary>
        /// The mode to allow users to specify to encrypt all extracts associated with specific published workbooks or data sources.
        /// </summary>
        public const string Enabled = "enabled";

        /// <summary>
        /// The mode to disable extract encryption on the site.
        /// </summary>
        public const string Disabled = "disabled";
    }
}
