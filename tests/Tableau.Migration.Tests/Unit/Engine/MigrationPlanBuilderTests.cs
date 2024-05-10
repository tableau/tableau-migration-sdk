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
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class MigrationPlanBuilderTests
    {
        public class MigrationPlanBuilderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationPlanOptionsBuilder> MockOptionsBuilder;
            protected readonly Mock<IMigrationHookBuilder> MockHookBuilder;
            protected readonly Mock<ContentMappingBuilder> MockMappingBuilder;
            protected readonly Mock<ContentFilterBuilder> MockFilterBuilder;
            protected readonly Mock<ContentTransformerBuilder> MockTransformerBuilder;
            protected readonly MigrationPlanBuilder Builder;

            public MigrationPlanBuilderTest()
            {
                MockOptionsBuilder = Create<Mock<IMigrationPlanOptionsBuilder>>();
                MockHookBuilder = Create<Mock<IMigrationHookBuilder>>();
                MockMappingBuilder = new() { CallBase = true };
                MockFilterBuilder = new() { CallBase = true };
                MockTransformerBuilder = new() { CallBase = true };

                Builder = new(
                    new TestSharedResourcesLocalizer(),
                    Create<Mock<ITableauApiSimulatorFactory>>().Object,
                    MockOptionsBuilder.Object,
                    MockHookBuilder.Object,
                    MockMappingBuilder.Object,
                    MockFilterBuilder.Object,
                    MockTransformerBuilder.Object);
            }

            protected void AssertExtensionsCleared()
            {
                MockHookBuilder.Verify(x => x.Clear(), Times.Once);
                MockMappingBuilder.Verify(x => x.Clear(), Times.Once);
                MockFilterBuilder.Verify(x => x.Clear(), Times.Once);
                MockTransformerBuilder.Verify(x => x.Clear(), Times.Once);
            }

            protected void AssertDefaultExtensions()
            {
                //Add expected default extensions to assert for here.

                Assert.All(PipelineProfile.ServerToCloud.GetSupportedContentTypes(),
                    ct => MockFilterBuilder.Verify(b => b.Add(typeof(PreviouslyMigratedFilter<>), It.IsAny<IEnumerable<Type[]>>()), Times.Once));

                MockFilterBuilder.Verify(b => b.Add<GroupAllUsersFilter, IGroup>(It.IsAny<Func<IServiceProvider, GroupAllUsersFilter>>()), Times.Once);
                MockFilterBuilder.Verify(b => b.Add(typeof(SystemOwnershipFilter<>), It.IsAny<IEnumerable<Type[]>>()), Times.Once);

                MockTransformerBuilder.Verify(x => x.Add<UserAuthenticationTypeTransformer, IUser>(It.IsAny<Func<IServiceProvider, UserAuthenticationTypeTransformer>>()), Times.Once);
                MockTransformerBuilder.Verify(x => x.Add<GroupUsersTransformer, IPublishableGroup>(It.IsAny<Func<IServiceProvider, GroupUsersTransformer>>()), Times.Once);
                MockTransformerBuilder.Verify(x => x.Add<TableauServerConnectionUrlTransformer, IPublishableWorkbook>(It.IsAny<Func<IServiceProvider, TableauServerConnectionUrlTransformer>>()), Times.Once);
            }

            protected void AssertDefaultServerToCloudExtensions()
            {
                AssertDefaultExtensions();

                //Add expected standard server-to-cloud extensions to assert for here.

                MockFilterBuilder.Verify(b => b.Add<UserSiteRoleSupportUserFilter, IUser>(It.IsAny<Func<IServiceProvider, UserSiteRoleSupportUserFilter>>()), Times.Once);

                MockTransformerBuilder.Verify(b => b.Add<UserTableauCloudSiteRoleTransformer, IUser>(It.IsAny<Func<IServiceProvider, UserTableauCloudSiteRoleTransformer>>()), Times.Once);
            }
        }

        public class ForServerToCloud : MigrationPlanBuilderTest
        {
            [Fact]
            public void InitializesForServerToCloud()
            {
                var builderResult = Builder.ForServerToCloud();

                Assert.NotSame(Builder, builderResult);
                Assert.IsType<ServerToCloudMigrationPlanBuilder>(builderResult);

                var plan = Builder.Build();

                Assert.Equal(PipelineProfile.ServerToCloud, plan.PipelineProfile);
                AssertExtensionsCleared();
                AssertDefaultExtensions();
            }

            [Fact]
            public void ReusesBuilderOnMultipleCalls()
            {
                var builderResult1 = Builder.ForServerToCloud();
                var builderResult2 = Builder.ForServerToCloud();

                Assert.Same(builderResult1, builderResult2);
            }
        }

        public class ClearExtensions : MigrationPlanBuilderTest
        {
            [Fact]
            public void ClearsExtensions()
            {
                Builder.ClearExtensions();
                AssertExtensionsCleared();
            }
        }

        public class AppendDefaultExtensions : MigrationPlanBuilderTest
        {
            [Fact]
            public void AddsExtensions()
            {
                var builderResult = Builder.AppendDefaultExtensions();

                Assert.Same(Builder, builderResult);

                AssertDefaultExtensions();
            }
        }

        public class Validate : MigrationPlanBuilderTest
        {
            private IMigrationPlanBuilder ConfigureBasicBuilder()
                => Builder
                    .FromSource(Create<TableauApiEndpointConfiguration>())
                    .ToDestination(Create<TableauApiEndpointConfiguration>());

            private IServerToCloudMigrationPlanBuilder ConfigureServerToCloudBuilder()
                => ConfigureBasicBuilder()
                    .ForServerToCloud();

            private IMigrationPlanBuilder ConfigureValidServerToCloudBuilder()
                => ConfigureServerToCloudBuilder()
                    .WithTableauCloudUsernames("salesforce.com")
                    .WithTableauIdAuthenticationType();

            [Fact]
            public void RequiresDefinedPipelineProfile()
            {
                var planBuilder = ConfigureBasicBuilder();

                var validationResult = planBuilder.Validate();

                validationResult.AssertFailure();
                var error = Assert.Single(validationResult.Errors);
                Assert.IsType<ValidationException>(error);
            }

            [Fact]
            public void SourceValidation()
            {
                var planBuilder = Builder
                    .ForServerToCloud()
                    .ToDestination(Create<TableauApiEndpointConfiguration>());

                var validationResult = planBuilder.Validate();

                validationResult.AssertFailure();
            }

            [Fact]
            public void DestinationValidation()
            {
                var planBuilder = Builder
                    .ForServerToCloud()
                    .FromSource(AutoFixture.Create<TableauApiEndpointConfiguration>());

                var validationResult = planBuilder.Validate();

                validationResult.AssertFailure();
            }

            [Fact]
            public void ValidatesServerToCloudRules()
            {
                var serverToCloudBuilder = ConfigureServerToCloudBuilder();

                var result = serverToCloudBuilder.Validate();

                result.AssertFailure();
            }

            [Fact]
            public void CorrectFilterContentType()
            {
                ConfigureValidServerToCloudBuilder()
                    .Filters.Add(Create<IContentFilter<IUser>>());

                var result = Builder.Validate();

                result.AssertSuccess();
            }

            [Fact]
            public void UnsupportedFilterContentType()
            {
                ConfigureValidServerToCloudBuilder()
                    .Filters.Add(Create<IContentFilter<TestContentType>>());

                var result = Builder.Validate();

                result.AssertFailure();
            }

            [Fact]
            public void CorrectMappingContentType()
            {
                ConfigureValidServerToCloudBuilder()
                    .Mappings.Add(Create<IContentMapping<IUser>>());

                var result = Builder.Validate();

                result.AssertSuccess();
            }

            [Fact]
            public void UnsupportedMappingContentType()
            {
                ConfigureValidServerToCloudBuilder()
                    .Mappings.Add(Create<IContentMapping<TestContentType>>());

                var result = Builder.Validate();

                result.AssertFailure();
            }

            [Fact]
            public void CorrectTransformerPublishType()
            {
                ConfigureValidServerToCloudBuilder()
                    .Transformers.Add(Create<IContentTransformer<IPublishableGroup>>());

                var result = Builder.Validate();

                result.AssertSuccess();
            }

            [Fact]
            public void UnsupportedTransformerPublishType()
            {
                ConfigureValidServerToCloudBuilder()
                    .Transformers.Add(Create<IContentTransformer<TestContentType>>());

                var result = Builder.Validate();

                result.AssertFailure();
            }

            [Fact]
            public void TransformerUsingContentTypeInsteadOfPublishType()
            {
                ConfigureValidServerToCloudBuilder()
                    .Transformers.Add(Create<IContentTransformer<IGroup>>());

                var result = Builder.Validate();

                result.AssertFailure();
            }
        }

        public class FromSource : MigrationPlanBuilderTest
        {
            [Fact]
            public void InitializesSource()
            {
                var source = AutoFixture.Create<IMigrationPlanEndpointConfiguration>();
                var builderResult = Builder.FromSource(source);

                Assert.Same(Builder, builderResult);

                var plan = Builder.Build();

                Assert.Same(source, plan.Source);
            }
        }

        public class FromSourceTableauServer : MigrationPlanBuilderTest
        {
            [Fact]
            public void SetsApiSourceAndReturns()
            {
                var server = new Uri("https://localhost");
                var site = "testSite";
                var tokenName = "myToken";
                var token = "tomen";

                var result = Builder.FromSourceTableauServer(server, site, tokenName, token);

                Assert.Same(Builder, result);

                var plan = Builder.Build();

                Assert.NotNull(plan.Source);
                var apiSource = Assert.IsType<TableauApiEndpointConfiguration>(plan.Source);
                Assert.Equal(server, apiSource.SiteConnectionConfiguration.ServerUrl);
                Assert.Equal(site, apiSource.SiteConnectionConfiguration.SiteContentUrl);
                Assert.Equal(tokenName, apiSource.SiteConnectionConfiguration.AccessTokenName);
                Assert.Equal(token, apiSource.SiteConnectionConfiguration.AccessToken);
            }
        }

        public class ToDestination : MigrationPlanBuilderTest
        {
            [Fact]
            public void InitializesDestination()
            {
                var destination = AutoFixture.Create<IMigrationPlanEndpointConfiguration>();
                var builderResult = Builder.ToDestination(destination);

                Assert.Same(Builder, builderResult);

                var plan = Builder.Build();

                Assert.Same(destination, plan.Destination);
            }
        }

        public class ToDestinationTableauCloud : MigrationPlanBuilderTest
        {
            [Fact]
            public void SetsApiDestinationAndReturns()
            {
                var cdNearUri = new Uri("https://cd-near.online.dev.tabint.net");

                var site = "testSite";
                var tokenName = "myToken";
                var token = "tomen";

                var result = Builder.ToDestinationTableauCloud(cdNearUri, site, tokenName, token);

                Assert.Same(Builder, result);

                var plan = Builder.Build();

                Assert.NotNull(plan.Destination);
                var apiDestination = Assert.IsType<TableauApiEndpointConfiguration>(plan.Destination);
                Assert.Equal(cdNearUri, apiDestination.SiteConnectionConfiguration.ServerUrl);
                Assert.Equal(site, apiDestination.SiteConnectionConfiguration.SiteContentUrl);
                Assert.Equal(tokenName, apiDestination.SiteConnectionConfiguration.AccessTokenName);
                Assert.Equal(token, apiDestination.SiteConnectionConfiguration.AccessToken);
            }
        }

        public class WithHooks : MigrationPlanBuilderTest
        {
            [Fact]
            public void AddsHooks()
            {
                var hookCollection = AutoFixture.Create<IMigrationHookFactoryCollection>();
                MockHookBuilder.Setup(x => x.Build()).Returns(hookCollection);
                var mappingCollection = AutoFixture.Create<IMigrationHookFactoryCollection>();
                MockMappingBuilder.Setup(x => x.Build()).Returns(mappingCollection);
                var filterCollection = AutoFixture.Create<IMigrationHookFactoryCollection>();
                MockFilterBuilder.Setup(x => x.Build()).Returns(filterCollection);
                var transformCollection = AutoFixture.Create<IMigrationHookFactoryCollection>();
                MockTransformerBuilder.Setup(x => x.Build()).Returns(transformCollection);

                var plan = Builder.ForServerToCloud()
                                  .Build();

                Assert.NotEqual(Guid.Empty, plan.PlanId);
                Assert.Equal(PipelineProfile.ServerToCloud, plan.PipelineProfile);
                Assert.Same(hookCollection, plan.Hooks);
                Assert.Same(mappingCollection, plan.Mappings);
                Assert.Same(filterCollection, plan.Filters);
                Assert.Same(transformCollection, plan.Transformers);
            }
        }

        public class Build : MigrationPlanBuilderTest
        {
            [Fact]
            public void BuildsPlan()
            {
                var hookCollection = AutoFixture.Create<IMigrationHookFactoryCollection>();
                MockHookBuilder.Setup(x => x.Build()).Returns(hookCollection);

                var plan = Builder.ForServerToCloud()
                    .Build();

                Assert.NotEqual(Guid.Empty, plan.PlanId);
                Assert.Equal(PipelineProfile.ServerToCloud, plan.PipelineProfile);
                Assert.Same(hookCollection, plan.Hooks);
            }
        }
    }
}
