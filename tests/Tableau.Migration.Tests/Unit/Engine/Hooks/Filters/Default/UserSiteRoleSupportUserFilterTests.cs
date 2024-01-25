using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class UserSiteRoleSupportUserFilterTests : AutoFixtureTestBase
    {
        public UserSiteRoleSupportUserFilterTests()
        { }

        public class ShouldMigrate : UserSiteRoleSupportUserFilterTests
        {
            [Fact]
            public void False_for_SupportUser()
            {
                var user = Create<ContentMigrationItem<IUser>>();
                user.SourceItem.SiteRole = SiteRoles.SupportUser;

                var filter = new UserSiteRoleSupportUserFilter();

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

                var filter = new UserSiteRoleSupportUserFilter();

                Assert.All(users, user => Assert.True(filter.ShouldMigrate(user)));
            }
        }
    }
}
