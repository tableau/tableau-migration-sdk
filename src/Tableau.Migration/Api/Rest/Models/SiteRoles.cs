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
    /// <para>
    /// Class containing site role constants.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/server/en-us/users_site_roles.htm#site-role-capabilities-summary">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class SiteRoles : StringEnum<SiteRoles>
    {
        /// <summary>
        /// Gets the name of the Creator site role.
        /// </summary>
        public const string Creator = "Creator";

        /// <summary>
        /// Gets the name of the Explorer site role.
        /// </summary>
        public const string Explorer = "Explorer";

        /// <summary>
        /// Gets the name of the Explorer Can Publish site role.
        /// </summary>
        public const string ExplorerCanPublish = "ExplorerCanPublish";

        /// <summary>
        /// Gets the name of the Guest site role.
        /// </summary>
        public const string Guest = "Guest";

        /// <summary>
        /// Gets the name of the Server Administrator site role.
        /// </summary>
        public const string ServerAdministrator = "ServerAdministrator";

        /// <summary>
        /// Gets the name of the Site Administrator Creator site role.
        /// </summary>
        public const string SiteAdministratorCreator = "SiteAdministratorCreator";

        /// <summary>
        /// Gets the name of the Site Administrator Explorer site role.
        /// </summary>
        public const string SiteAdministratorExplorer = "SiteAdministratorExplorer";

        /// <summary>
        /// Gets the name of the Support User site role.
        /// </summary>
        public const string SupportUser = "SupportUser";

        /// <summary>
        /// Gets the name of the Unlicensed site role.
        /// </summary>
        public const string Unlicensed = "Unlicensed";

        /// <summary>
        /// Gets the name of the Viewer site role.
        /// </summary>
        public const string Viewer = "Viewer";
    }
}

