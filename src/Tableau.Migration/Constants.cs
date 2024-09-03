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

using System.Text;

namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing Tableau Migration SDK constant values.
    /// </summary>
    public static class Constants
    {
        #region - Public Constants -

        /// <summary>
        /// The separator to follow a domain name when a domain name is included in entity names.
        /// </summary>
        public const string DomainNameSeparator = "\\";

        /// <summary>
        /// The default path separator for <see cref="ContentLocation"/>s.
        /// </summary>
        public const string PathSeparator = "/";

        /// <summary>
        /// The local domain name.
        /// </summary>
        public const string LocalDomain = "local";

        /// <summary>
        /// The external domain name.
        /// </summary>
        public const string ExternalDomain = "external";

        /// <summary>
        /// The domain name for the Tableau ID with MFA authentication type.
        /// </summary>
        public const string TableauIdWithMfaDomain = "TABID_WITH_MFA";

        /// <summary>
        /// The username for the built-in system user.
        /// </summary>
        public const string SystemUsername = "_system";

        /// <summary>
        /// The location for the local system user.
        /// </summary>
        public static readonly ContentLocation SystemUserLocation = ContentLocation.ForUsername(LocalDomain, SystemUsername);

        /// <summary>
        /// The name for the built-in default project.
        /// </summary>
        public const string DefaultProjectName = "Default";

        /// <summary>
        /// The name for the built-in admin insights project.
        /// </summary>
        public const string AdminInsightsProjectName = "Admin Insights";

        /// <summary>
        /// Alternate name for the built-in admin insights project.
        /// </summary>
        public const string AdminInsightsTableauProjectName = "Admin Insights (Tableau)";

        /// <summary>
        /// Alternate  name for the built-in admin insights project.
        /// </summary>
        public const string AdminInsightsTableauOnlineProjectName = "Admin Insights (Tableau Online)";

        /// <summary>
        /// Schedules frequency format (<see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_extract_and_encryption.htm#create_cloud_extract_refresh_task">reference</see>).
        /// </summary>
        public const string FrequencyTimeFormat = "HH:mm:ss";

        /// <summary>
        /// The default text encoding.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        #endregion

        #region - Internal Constants -

        /// <summary>
        /// The comment for the python user agent.
        /// </summary>
        internal const string PYTHON_USER_AGENT_COMMENT = "Python";

        /// <summary>
        /// The comment for the python user agent.
        /// </summary>
        internal const string PYTHON_ENVIRONMENT_VARIABLE_PREFIX = "MigrationSDK__";

        /// <summary>
        /// The comment for the python user agent.
        /// </summary>
        internal const string PYTHON_USER_AGENT_COMMENT_CONFIG_KEY = PYTHON_ENVIRONMENT_VARIABLE_PREFIX + "Network__UserAgentComment";

        /// <summary>
        /// The default prefix for the user agent string.
        /// </summary>
        internal const string USER_AGENT_PREFIX = "TableauMigrationSDK";

        #endregion
    }
}
