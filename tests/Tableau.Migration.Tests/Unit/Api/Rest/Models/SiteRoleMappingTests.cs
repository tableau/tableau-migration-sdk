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

using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class SiteRoleMappingTests
    {
        public class GetLicenseLevel
        {
            [Theory]
            [InlineData(SiteRoles.Explorer, LicenseLevels.Explorer)]
            [InlineData(SiteRoles.ExplorerCanPublish, LicenseLevels.Explorer)]
            [InlineData(SiteRoles.SiteAdministratorCreator, LicenseLevels.Creator)]
            [InlineData(SiteRoles.ServerAdministrator, LicenseLevels.Creator)]
            [InlineData(SiteRoles.Creator, LicenseLevels.Creator)]
            [InlineData(SiteRoles.SiteAdministratorExplorer, LicenseLevels.Explorer)]
            [InlineData(SiteRoles.Unlicensed, LicenseLevels.Unlicensed)]
            [InlineData(SiteRoles.Viewer, LicenseLevels.Viewer)]
            [InlineData(SiteRoles.Guest, LicenseLevels.Viewer)]
            [InlineData(SiteRoles.SupportUser, LicenseLevels.Viewer)]
            public void MapsSiteRole(string siteRole, string expectedlicenseLevel)
                => Assert.Equal(SiteRoleMapping.GetLicenseLevel(siteRole), expectedlicenseLevel);

            [Fact]
            public void Reuturns_Unlicensed_on_empty_input()
                => Assert.Equal(LicenseLevels.Unlicensed, SiteRoleMapping.GetLicenseLevel(string.Empty));
        }
        public class GetAdministratorLevel
        {
            [Theory]
            [InlineData(SiteRoles.SiteAdministratorExplorer, AdministratorLevels.Site)]
            [InlineData(SiteRoles.SiteAdministratorCreator, AdministratorLevels.Site)]
            [InlineData(SiteRoles.ServerAdministrator, AdministratorLevels.Site)]
            [InlineData(SiteRoles.Creator, AdministratorLevels.None)]
            [InlineData(SiteRoles.Explorer, AdministratorLevels.None)]
            [InlineData(SiteRoles.ExplorerCanPublish, AdministratorLevels.None)]
            [InlineData(SiteRoles.Viewer, AdministratorLevels.None)]
            [InlineData(SiteRoles.Unlicensed, AdministratorLevels.None)]
            [InlineData(SiteRoles.Guest, AdministratorLevels.None)]
            [InlineData(SiteRoles.SupportUser, AdministratorLevels.None)]
            public void MapsSiteRole(string siteRole, string expectedlicenseLevel)
                => Assert.Equal(SiteRoleMapping.GetAdministratorLevel(siteRole), expectedlicenseLevel);

            [Fact]
            public void Reuturns_None_on_empty_input()
                => Assert.Equal(AdministratorLevels.None, SiteRoleMapping.GetAdministratorLevel(string.Empty));
        }

        public class GetPublishingCapability
        {
            [Theory]
            [InlineData(SiteRoles.SiteAdministratorExplorer, true)]
            [InlineData(SiteRoles.SiteAdministratorCreator, true)]
            [InlineData(SiteRoles.ServerAdministrator, true)]
            [InlineData(SiteRoles.Creator, true)]
            [InlineData(SiteRoles.ExplorerCanPublish, true)]
            [InlineData(SiteRoles.Explorer, false)]
            [InlineData(SiteRoles.Viewer, false)]
            [InlineData(SiteRoles.Unlicensed, false)]
            [InlineData(SiteRoles.Guest, false)]
            [InlineData(SiteRoles.SupportUser, false)]
            public void MapsSiteRole(string siteRole, bool expectedCapability)
                => Assert.Equal(SiteRoleMapping.GetPublishingCapability(siteRole), expectedCapability);

            [Fact]
            public void Returns_False_on_empty_input()
                => Assert.False(SiteRoleMapping.GetPublishingCapability(string.Empty));

        }

        public class GetSiteRole
        {
            [Theory]
            [InlineData(SiteRoles.SiteAdministratorExplorer, AdministratorLevels.Site, LicenseLevels.Explorer, true)]
            [InlineData(SiteRoles.SiteAdministratorCreator, AdministratorLevels.Site, LicenseLevels.Creator, true)]
            [InlineData(SiteRoles.Creator, AdministratorLevels.None, LicenseLevels.Creator, true)]
            [InlineData(SiteRoles.Explorer, AdministratorLevels.None, LicenseLevels.Explorer)]
            [InlineData(SiteRoles.ExplorerCanPublish, AdministratorLevels.None, LicenseLevels.Explorer, true)]
            [InlineData(SiteRoles.Viewer, AdministratorLevels.None, LicenseLevels.Viewer)]
            [InlineData(SiteRoles.Unlicensed, AdministratorLevels.None, LicenseLevels.Unlicensed)]
            public void Parses(string expectedSiteRole, string adminLevel, string licenseLevel, bool canPublish = false)
                => Assert.Equal(expectedSiteRole, SiteRoleMapping.GetSiteRole(adminLevel, licenseLevel, canPublish));

            [Fact]
            public void Returns_Unlicensed_on_empty_input()
                => Assert.Equal(SiteRoles.Unlicensed, SiteRoleMapping.GetSiteRole(string.Empty, string.Empty, false));
        }
    }
}
