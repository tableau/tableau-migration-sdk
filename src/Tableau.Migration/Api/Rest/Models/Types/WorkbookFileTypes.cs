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

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// <para>
    /// Class containing workbook file type constants.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_publishing.htm#publish_workbook">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class WorkbookFileTypes : StringEnum<WorkbookFileTypes>
    {
        /// <summary>
        /// Gets the name of the twb workbook file type.
        /// </summary>
        public const string Twb = "twb";

        /// <summary>
        /// Gets the name of the twbx workbook file type.
        /// </summary>
        public const string Twbx = "twbx";
    }
}

