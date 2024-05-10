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
    /// Class containing content permissions constants. 
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_projects.htm#create_project">Tableau API Reference</see> 
    /// for documentation.
    /// </summary>
    public class ContentPermissions : StringEnum<ContentPermissions>
    {
        /// <summary>
        /// Gets the name of the LockedToProject content permission mode.
        /// </summary>
        public const string LockedToProject = "LockedToProject";

        /// <summary>
        /// Gets the name of the ManagedByOwner content permission mode.
        /// </summary>
        public const string ManagedByOwner = "ManagedByOwner";

        /// <summary>
        /// Gets the name of the LockedToProjectWithoutNested content permission mode.
        /// </summary>
        public const string LockedToProjectWithoutNested = "LockedToProjectWithoutNested";
    }
}
