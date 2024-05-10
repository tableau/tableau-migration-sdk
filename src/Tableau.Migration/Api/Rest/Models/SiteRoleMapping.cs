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

using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Class containing mappings of a user's <see cref="SiteRoles"/>, <see cref="AdministratorLevels"/>, <see cref="LicenseLevels"/>
    /// and Publishing Capability (boolean).
    /// </summary>
    public static class SiteRoleMapping
    {
        /// <summary>
        /// Mappings of a user's <see cref="SiteRoles"/>, <see cref="AdministratorLevels"/>, <see cref="LicenseLevels"/>
        /// and Publishing Capability (boolean).
        /// <list type="table">
        /// <listheader>
        /// <term>Site Role</term><term>Administrator Level</term><term>License Level</term><term>Publishing Capability</term>
        /// </listheader>
        /// <item><term><see cref='SiteRoles.Creator'/></term><term><see cref='AdministratorLevels.None'/></term><term><see cref='LicenseLevels.Creator'/></term> <term>TRUE</term></item>
        /// <item><term><see cref='SiteRoles.Explorer'/></term><term><see cref='AdministratorLevels.None'/></term><term><see cref='LicenseLevels.Explorer'/></term> <term>FALSE</term></item>
        /// <item><term><see cref='SiteRoles.ExplorerCanPublish'/></term><term><see cref='AdministratorLevels.None'/></term><term><see cref='LicenseLevels.Explorer'/></term> <term>TRUE</term></item>
        /// <item><term><see cref='SiteRoles.Guest'/></term><term><see cref='AdministratorLevels.None'/></term><term><see cref='LicenseLevels.Viewer'/></term> <term>FALSE</term></item>
        /// <item><term><see cref='SiteRoles.SiteAdministratorCreator'/></term><term><see cref='AdministratorLevels.Site'/></term><term><see cref='LicenseLevels.Creator'/></term> <term>TRUE</term></item>
        /// <item><term><see cref='SiteRoles.ServerAdministrator'/></term><term><see cref='AdministratorLevels.Site'/></term><term><see cref='LicenseLevels.Creator'/></term> <term>TRUE</term></item>
        /// <item><term><see cref='SiteRoles.SiteAdministratorExplorer'/></term><term><see cref='AdministratorLevels.Site'/></term><term><see cref='LicenseLevels.Explorer'/></term> <term>TRUE</term></item>
        /// <item><term><see cref='SiteRoles.SupportUser'/></term><term><see cref='AdministratorLevels.None'/></term><term><see cref='LicenseLevels.Viewer'/></term> <term>FALSE</term></item>
        /// <item><term><see cref='SiteRoles.Unlicensed'/></term><term><see cref='AdministratorLevels.None'/></term><term><see cref='LicenseLevels.Unlicensed'/></term> <term>FALSE</term></item>
        /// <item><term><see cref='SiteRoles.Viewer'/></term><term><see cref='AdministratorLevels.None'/></term><term><see cref='LicenseLevels.Viewer'/></term> <term>FALSE</term></item>   
        /// </list>
        /// </summary>
        public static readonly List<SiteRoleMappingItem> Collection = new()
        {
            new SiteRoleMappingItem(SiteRoles.SiteAdministratorExplorer, AdministratorLevels.Site, LicenseLevels.Explorer, true),
            new SiteRoleMappingItem(SiteRoles.SiteAdministratorCreator, AdministratorLevels.Site, LicenseLevels.Creator, true),
            new SiteRoleMappingItem(SiteRoles.ServerAdministrator, AdministratorLevels.Site, LicenseLevels.Creator, true),
            new SiteRoleMappingItem(SiteRoles.Creator, AdministratorLevels.None, LicenseLevels.Creator, true),
            new SiteRoleMappingItem(SiteRoles.Explorer, AdministratorLevels.None, LicenseLevels.Explorer,false),
            new SiteRoleMappingItem(SiteRoles.ExplorerCanPublish, AdministratorLevels.None, LicenseLevels.Explorer, true),
            new SiteRoleMappingItem(SiteRoles.Viewer, AdministratorLevels.None, LicenseLevels.Viewer,false),
            new SiteRoleMappingItem(SiteRoles.SupportUser, AdministratorLevels.None, LicenseLevels.Viewer, false),
            new SiteRoleMappingItem(SiteRoles.Guest, AdministratorLevels.None, LicenseLevels.Viewer,false),
            new SiteRoleMappingItem(SiteRoles.Unlicensed, AdministratorLevels.None, LicenseLevels.Unlicensed,false),
        };

        /// <summary>
        /// Map a user's SiteRole to a user's administrator level from <see cref="AdministratorLevels"/>.
        /// </summary>
        /// <param name="siteRole">The siterole of the user.</param>
        /// <returns>
        /// Administrator Level from <see cref="AdministratorLevels"/>. 
        /// Default is <see cref="AdministratorLevels.None"/>. 
        /// </returns>
        public static string GetAdministratorLevel(string? siteRole)
        => Collection.FirstOrDefault(m => SiteRoles.IsAMatch(siteRole, m.SiteRole))?.AdministratorLevel
            ?? AdministratorLevels.None;

        /// <summary>
        /// Map a user's SiteRole to a user's license level from <see cref="LicenseLevels"/>.
        /// </summary>
        /// <param name="siteRole"></param>
        /// <returns>
        /// License Levels from <see cref="LicenseLevels"/>. 
        /// Default is <see cref="LicenseLevels.Unlicensed"/>.
        /// </returns>
        public static string GetLicenseLevel(string? siteRole)
            => Collection.FirstOrDefault(m => SiteRoles.IsAMatch(siteRole, m.SiteRole))?.LicenseLevel
            ?? LicenseLevels.Unlicensed;

        /// <summary>
        /// Map a user's siterole to their publishing capability (boolean).
        /// </summary>
        /// <param name="siteRole">The SiteRole of the user.</param>
        /// <returns>
        /// The Publishing Capability of the user. Default is false.
        /// </returns>
        public static bool GetPublishingCapability(string? siteRole)
            => Collection.FirstOrDefault(m => SiteRoles.IsAMatch(siteRole, m.SiteRole))?.CanPublish
            ?? false;

        /// <summary>
        /// Map input parameter values to a user's SiteRole.
        /// </summary>
        /// <param name="adminLevel">One of the <see cref="AdministratorLevels"/> of the user.</param>
        /// <param name="licenseLevel">One of the <see cref="LicenseLevels"/> of the user.</param>
        /// <param name="canPublish">The Publishing Capability of the user.</param>
        /// <returns>A SiteRole from <see cref="SiteRoles"/>.</returns>
        public static string GetSiteRole(string adminLevel, string licenseLevel, bool canPublish)
            => Collection.FirstOrDefault(
                m => AdministratorLevels.IsAMatch(adminLevel, m.AdministratorLevel) &&
                m.CanPublish == canPublish &&
                LicenseLevels.IsAMatch(licenseLevel, m.LicenseLevel))
                ?.SiteRole ?? SiteRoles.Unlicensed;
    }
}
