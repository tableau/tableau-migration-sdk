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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
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

        private readonly MockSharedResourcesLocalizer _mockLocalizer = new();
        private readonly Mock<ILogger<UserTableauCloudSiteRoleTransformer>> _mockLogger = new();

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
            var siteRoleTransformer = new UserTableauCloudSiteRoleTransformer(_mockLocalizer.Object, _mockLogger.Object);

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
            var siteRoleTransformer = new UserTableauCloudSiteRoleTransformer(_mockLocalizer.Object, _mockLogger.Object);

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
            var siteRoleTransformer = new UserTableauCloudSiteRoleTransformer(_mockLocalizer.Object, _mockLogger.Object);

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
