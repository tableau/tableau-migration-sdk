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

namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// <para>
    /// Class containing default project permission content type URL segment constants.
    /// </para>
    /// <para>
    /// For example, for the URL "/api/api-version/sites/site-luid/projects/project-luid/default-permissions/workbooks" the URL segment would be "workbooks".
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_permissions.htm#query_default_permissions">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class DefaultPermissionsContentTypeUrlSegments : StringEnum<DefaultPermissionsContentTypeUrlSegments>
    {
        /// <summary>
        /// Gets the workbook content type URL path segment.
        /// </summary>
        public const string Workbooks = "workbooks";

        /// <summary>
        /// Gets the data source content type URL path segment.
        /// </summary>
        public const string DataSources = "datasources";

        /// <summary>
        /// Gets the flow content type URL path segment.
        /// </summary>
        public const string Flows = "flows";

        /// <summary>
        /// Gets the metric content type URL path segment.
        /// </summary>
        public const string Metrics = "metrics";

        /// <summary>
        /// Gets the database content type URL path segment.
        /// </summary>
        public const string Databases = "databases";

        /// <summary>
        /// Gets the table content type URL path segment.
        /// </summary>
        public const string Tables = "tables";
    }
}
