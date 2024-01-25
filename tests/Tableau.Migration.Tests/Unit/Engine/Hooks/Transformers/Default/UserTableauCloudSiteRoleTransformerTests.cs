using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class UserTableauCloudSiteRoleTransformerTests : AutoFixtureTestBase
    {
        private readonly List<ContentMigrationItem<IUser>> _transformerExecutionContexts;
        private readonly List<IMigrationHookFactory> _transformerFactories;

        private readonly IMigrationPlan _plan;

        private readonly ContentTransformerRunner _runner;

        public UserTableauCloudSiteRoleTransformerTests()
        {
            _transformerExecutionContexts = new();
            _transformerFactories = new();

            var mockTransformers = AutoFixture.Create<Mock<IMigrationHookFactoryCollection>>();
            mockTransformers.Setup(x => x.GetHooks<IContentTransformer<IUser>>()).Returns(() => _transformerFactories.ToImmutableArray());

            var mockPlan = AutoFixture.Create<Mock<IMigrationPlan>>();
            mockPlan.SetupGet(x => x.Transformers).Returns(mockTransformers.Object);
            _plan = mockPlan.Object;
            Assert.NotNull(_plan.Transformers);
            _runner = new(_plan, new Mock<IServiceProvider>().Object);
        }

        [Theory]

        [InlineData(SiteRoles.ServerAdministrator, SiteRoles.SiteAdministratorCreator)]
        [InlineData("serverAdministrator", SiteRoles.SiteAdministratorCreator)]
        [InlineData("Serveradministrator", SiteRoles.SiteAdministratorCreator)]
        public async Task Transforms_server_admin_role(string siteRole, string expectedSiteRole)
        {
            // Arrange            
            var siteRoleTransformer = new UserTableauCloudSiteRoleTransformer();

            var input = AutoFixture.Create<IUser>();
            input.SiteRole = siteRole;

            _transformerFactories.Add(new MigrationHookFactory(s => siteRoleTransformer));

            // Act
            var result = await _runner.ExecuteAsync(input, default);

            // Assert
            Assert.Equal(expectedSiteRole, result.SiteRole);
        }

        [Theory]
        [InlineData("", "")]
        public async Task Handles_Blank(string siteRole, string expectedSiteRole)
        {
            // Arrange            
            var siteRoleTransformer = new UserTableauCloudSiteRoleTransformer();

            var input = AutoFixture.Create<IUser>();
            input.SiteRole = siteRole;

            _transformerFactories.Add(new MigrationHookFactory(s => siteRoleTransformer));

            // Act
            var result = await _runner.ExecuteAsync(input, default);

            // Assert
            Assert.Equal(expectedSiteRole, result.SiteRole);
        }

        [Theory]


        [InlineData(SiteRoles.Creator)]
        [InlineData(SiteRoles.Explorer)]
        [InlineData(SiteRoles.ExplorerCanPublish)]
        [InlineData(SiteRoles.Guest)]
        [InlineData(SiteRoles.SiteAdministratorCreator)]
        [InlineData(SiteRoles.SiteAdministratorExplorer)]
        [InlineData(SiteRoles.SupportUser)]
        [InlineData(SiteRoles.Unlicensed)]
        [InlineData(SiteRoles.Viewer)]
        public async Task Ignores_otherRoles(string siteRole)
        {
            // Arrange            
            var siteRoleTransformer = new UserTableauCloudSiteRoleTransformer();

            var input = AutoFixture.Create<IUser>();
            input.SiteRole = siteRole;

            _transformerFactories.Add(new MigrationHookFactory(s => siteRoleTransformer));

            // Act
            var result = await _runner.ExecuteAsync<IUser>(input, default);

            // Assert
            Assert.Equal(siteRole, result.SiteRole);
        }
    }
}
