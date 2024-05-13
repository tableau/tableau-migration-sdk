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
    /// Class containing data source file type constants.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#publish_data_source">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class DataSourceFileTypes : StringEnum<DataSourceFileTypes>
    {
        /// <summary>
        /// Gets the name of the Hyper data source file type.
        /// </summary>
        public const string Hyper = "hyper";

        /// <summary>
        /// Gets the name of the Tds data source file type.
        /// </summary>
        public const string Tds = "tds";

        /// <summary>
        /// Gets the name of the Tdsx data source file type.
        /// </summary>
        public const string Tdsx = "tdsx";

        /// <summary>
        /// Gets the name of the Tde data source file type.
        /// </summary>
        public const string Tde = "tde";
    }
}

