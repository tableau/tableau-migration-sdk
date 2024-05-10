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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tableau.Migration.Engine.Hooks.Filters;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class UserSiteRoleSupportUserFilterTests : AutoFixtureTestBase
    {
        MockSharedResourcesLocalizer _mockLocalizer = new();
        Mock<ILogger<IContentFilter<IUser>>> _mockLogger = new();

        public UserSiteRoleSupportUserFilterTests()
        { }

        public class ShouldMigrate : UserSiteRoleSupportUserFilterTests
        {
            [Fact]
            public void False_for_SupportUser()
            {
                var user = Create<ContentMigrationItem<IUser>>();
                user.SourceItem.SiteRole = SiteRoles.SupportUser;

                var filter = new UserSiteRoleSupportUserFilter(_mockLocalizer.Object, _mockLogger.Object);

                Assert.False(filter.ShouldMigrate(user));
            }

            [Fact]
            public void True_for_every_role_except_SupportUser()
            {
                var users = new List<ContentMigrationItem<IUser>>();

                var siteRoles = SiteRoles.GetAll().Where(r => r != SiteRoles.SupportUser).ToList();

                for (int i = 0; i < 20; i++)
                {
                    var user = Create<ContentMigrationItem<IUser>>();

                    user.SourceItem.SiteRole = siteRoles[i % siteRoles.Count];

                    users.Add(user);
                }

                var filter = new UserSiteRoleSupportUserFilter(_mockLocalizer.Object, _mockLogger.Object);

                Assert.All(users, user => Assert.True(filter.ShouldMigrate(user)));
            }
        }
    }
}
