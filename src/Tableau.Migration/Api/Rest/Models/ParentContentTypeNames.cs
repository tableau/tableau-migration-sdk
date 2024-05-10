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
    /// Class containing ParentContentTypeName constants for use in permissions.
    /// </summary>
    public class ParentContentTypeNames : StringEnum<ParentContentTypeNames>
    {
        /// <summary>
        /// Gets the name of data source parent content type.
        /// </summary>
        public const string DataSource = "DataSource";

        /// <summary>
        /// Gets the name of flow parent content type.
        /// </summary>
        public const string Flow = "Flow";

        /// <summary>
        /// Gets the name of project parent content type.
        /// </summary>
        public const string Project = "Project";

        /// <summary>
        /// Gets the name of workbook parent content type.
        /// </summary>
        public const string Workbook = "Workbook";
    }
}
