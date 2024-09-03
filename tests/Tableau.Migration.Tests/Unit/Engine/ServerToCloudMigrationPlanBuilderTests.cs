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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class ServerToCloudMigrationPlanBuilderTests
    {
        public class ServerToCloudMigrationPlanBuilderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationPlanBuilder> MockInnerBuilder;
            protected readonly ServerToCloudMigrationPlanBuilder Builder;

            public ServerToCloudMigrationPlanBuilderTest()
            {
                MockInnerBuilder = Create<Mock<IMigrationPlanBuilder>>();
                Builder = new(new TestSharedResourcesLocalizer(), MockInnerBuilder.Object);
            }

            protected void AssertRequiredAuthTypeExtensions(string authType, string userDomain, string groupDomain)
            {
                MockInnerBuilder.Verify(x => x.Mappings.Add<AuthenticationTypeDomainMapping, IUser>(null), Times.Once);
                MockInnerBuilder.Verify(x => x.Mappings.Add<AuthenticationTypeDomainMapping, IGroup>(null), Times.Once);
                MockInnerBuilder.Verify(x =>
                    x.Options.Configure(It.Is<AuthenticationTypeDomainMappingOptions>(o => o.UserDomain == userDomain && o.GroupDomain == groupDomain)),
                    Times.Once);

                MockInnerBuilder.Verify(x =>
                    x.Options.Configure(It.Is<UserAuthenticationTypeTransformerOptions>(o => o.AuthenticationType == authType)),
                    Times.Once);
            }

            protected void AssertRequiredUsernameExtensions(string mailDomain, bool useExistingEmail)
            {
                MockInnerBuilder.Verify(x => x.Mappings.Add<TableauCloudUsernameMapping, IUser>(null), Times.Once);

                MockInnerBuilder.Verify(x =>
                    x.Options.Configure(It.Is<TableauCloudUsernameMappingOptions>(
                            o => o.MailDomain == mailDomain && o.UseExistingEmail == useExistingEmail
                    )), Times.Once);
            }
        }

        #region - IMigrationPlanBuilder Wrapping -

        public class Wrapper : ServerToCloudMigrationPlanBuilderTest
        {
            [Fact]
            public void Options()
            {
                var o = ((IMigrationPlanBuilder)Builder).Options;

                MockInnerBuilder.VerifyGet(x => x.Options, Times.Once);
            }

            [Fact]
            public void Hooks()
            {
                var h = ((IMigrationPlanBuilder)Builder).Hooks;

                MockInnerBuilder.VerifyGet(x => x.Hooks, Times.Once);
            }

            [Fact]
            public void Mappings()
            {
                var m = ((IMigrationPlanBuilder)Builder).Mappings;

                MockInnerBuilder.VerifyGet(x => x.Mappings, Times.Once);
            }

            [Fact]
            public void Filters()
            {
                var f = ((IMigrationPlanBuilder)Builder).Filters;

                MockInnerBuilder.VerifyGet(x => x.Filters, Times.Once);
            }

            [Fact]
            public void Transformers()
            {
                var t = ((IMigrationPlanBuilder)Builder).Transformers;

                MockInnerBuilder.VerifyGet(x => x.Transformers, Times.Once);
            }

            [Fact]
            public void Build()
            {
                var p = ((IMigrationPlanBuilder)Builder).Build();

                MockInnerBuilder.Verify(x => x.Build(), Times.Once);
            }

            [Fact]
            public void ClearExtensions()
            {
                var b = ((IMigrationPlanBuilder)Builder).ClearExtensions();

                MockInnerBuilder.Verify(x => x.ClearExtensions(), Times.Once);
            }

            [Fact]
            public void AppendDefaultExtensions()
            {
                var b = ((IMigrationPlanBuilder)Builder).AppendDefaultExtensions();

                MockInnerBuilder.Verify(x => x.AppendDefaultExtensions(), Times.Once);
            }

            [Fact]
            public void ForServerToCloud()
            {
                var b = ((IMigrationPlanBuilder)Builder).ForServerToCloud();

                MockInnerBuilder.Verify(x => x.ForServerToCloud(), Times.Once);
            }

            private IMigrationPipelineFactory CreateFactory(IServiceProvider services) => Create<IMigrationPipelineFactory>();

            [Fact]
            public void ForCustomPipelineFactory()
            {
                var contentTypes = CreateMany<MigrationPipelineContentType>();
                var contentTypesArray = contentTypes.ToArray();

                var b = ((IMigrationPlanBuilder)Builder).ForCustomPipelineFactory<MigrationPipelineFactory>(contentTypesArray);
                b = ((IMigrationPlanBuilder)Builder).ForCustomPipelineFactory<MigrationPipelineFactory>(contentTypes);

                b = ((IMigrationPlanBuilder)Builder).ForCustomPipelineFactory(CreateFactory, contentTypesArray);
                b = ((IMigrationPlanBuilder)Builder).ForCustomPipelineFactory(CreateFactory, contentTypes);

                MockInnerBuilder.Verify(x => x.ForCustomPipelineFactory<MigrationPipelineFactory>(contentTypesArray), Times.Once);
                MockInnerBuilder.Verify(x => x.ForCustomPipelineFactory<MigrationPipelineFactory>(contentTypes), Times.Once);
                MockInnerBuilder.Verify(x => x.ForCustomPipelineFactory(CreateFactory, contentTypesArray), Times.Once);
                MockInnerBuilder.Verify(x => x.ForCustomPipelineFactory(CreateFactory, contentTypes), Times.Once);
            }

            [Fact]
            public void ForCustomPipeline()
            {
                var contentTypes = CreateMany<MigrationPipelineContentType>();
                var contentTypesArray = contentTypes.ToArray();

                var b = ((IMigrationPlanBuilder)Builder).ForCustomPipeline<ServerToCloudMigrationPipeline>(contentTypesArray);
                b = ((IMigrationPlanBuilder)Builder).ForCustomPipeline<ServerToCloudMigrationPipeline>(contentTypes);

                MockInnerBuilder.Verify(x => x.ForCustomPipeline<ServerToCloudMigrationPipeline>(contentTypesArray), Times.Once);
                MockInnerBuilder.Verify(x => x.ForCustomPipeline<ServerToCloudMigrationPipeline>(contentTypes), Times.Once);
            }

            [Fact]
            public void FromSource()
            {
                var b = ((IMigrationPlanBuilder)Builder)
                    .FromSource(Create<Mock<IMigrationPlanEndpointConfiguration>>().Object);

                MockInnerBuilder.Verify(x => x.FromSource(It.IsAny<IMigrationPlanEndpointConfiguration>()), Times.Once);
            }

            [Fact]
            public void ToDestination()
            {
                var b = ((IMigrationPlanBuilder)Builder)
                    .ToDestination(Create<Mock<IMigrationPlanEndpointConfiguration>>().Object);

                MockInnerBuilder.Verify(x => x.ToDestination(It.IsAny<IMigrationPlanEndpointConfiguration>()), Times.Once);
            }

            [Fact]
            public void Validate()
            {
                var r = ((IMigrationPlanBuilder)Builder).Validate();

                MockInnerBuilder.Verify(x => x.Validate(), Times.Once);
            }
        }

        #endregion

        #region - WithAuthenticationType (and related) -

        public class WithSamlAuthenticationType : ServerToCloudMigrationPlanBuilderTest
        {
            [Fact]
            public void RegistersSamlDomainMapping()
            {
                Builder.WithSamlAuthenticationType("myDomain");

                AssertRequiredAuthTypeExtensions(AuthenticationTypes.Saml, "myDomain", Constants.LocalDomain);
            }
        }

        public class WithTableauIdAuthenticationType : ServerToCloudMigrationPlanBuilderTest
        {
            [Fact]
            public void WithMfa()
            {
                Builder.WithTableauIdAuthenticationType();

                AssertRequiredAuthTypeExtensions(AuthenticationTypes.TableauIdWithMfa, Constants.TableauIdWithMfaDomain, Constants.LocalDomain);
            }

            [Fact]
            public void WithoutMfa()
            {
                Builder.WithTableauIdAuthenticationType(false);

                AssertRequiredAuthTypeExtensions(AuthenticationTypes.OpenId, Constants.ExternalDomain, Constants.LocalDomain);
            }
        }

        public class WithAuthenticationType : ServerToCloudMigrationPlanBuilderTest
        {
            [Fact]
            public void WithAuthTypeAndDomain()
            {
                Builder.WithAuthenticationType("myAuthType", "userDomain", "groupDomain");

                AssertRequiredAuthTypeExtensions("myAuthType", "userDomain", "groupDomain");
            }

            [Fact]
            public void WithObject()
            {
                var myMapping = Create<Mock<IAuthenticationTypeDomainMapping>>().Object;

                Builder.WithAuthenticationType("myAuthType", myMapping);

                MockInnerBuilder.Verify(x => x.Mappings.Add<IUser>(myMapping), Times.Once);
                MockInnerBuilder.Verify(x => x.Mappings.Add<IGroup>(myMapping), Times.Once);


                MockInnerBuilder.Verify(x =>
                    x.Options.Configure(It.Is<UserAuthenticationTypeTransformerOptions>(o => o.AuthenticationType == "myAuthType")),
                    Times.Once);
            }

            private class TestAuthTypeDomainMapping : IAuthenticationTypeDomainMapping
            {
                public Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
                    => ctx.ToTask();

                public Task<ContentMappingContext<IGroup>?> ExecuteAsync(ContentMappingContext<IGroup> ctx, CancellationToken cancel)
                    => ctx.ToTask();
            }

            [Fact]
            public void WithFactory()
            {
                var fact = (IServiceProvider services) => new TestAuthTypeDomainMapping();

                Builder.WithAuthenticationType<TestAuthTypeDomainMapping>("myAuthType", fact);

                MockInnerBuilder.Verify(x => x.Mappings.Add<TestAuthTypeDomainMapping, IUser>(fact), Times.Once);
                MockInnerBuilder.Verify(x => x.Mappings.Add<TestAuthTypeDomainMapping, IGroup>(fact), Times.Once);


                MockInnerBuilder.Verify(x =>
                    x.Options.Configure(It.Is<UserAuthenticationTypeTransformerOptions>(o => o.AuthenticationType == "myAuthType")),
                    Times.Once);
            }

            [Fact]
            public void WithCallback()
            {
                var callback = (ContentMappingContext<IUsernameContent> ctx, CancellationToken cancel) =>
                    Task.FromResult("myDomain");

                Builder.WithAuthenticationType("myAuthType", callback);

                MockInnerBuilder.Verify(x => x.Mappings.Add<IUser>(It.IsAny<IAuthenticationTypeDomainMapping>()), Times.Once);
                MockInnerBuilder.Verify(x => x.Mappings.Add<IGroup>(It.IsAny<IAuthenticationTypeDomainMapping>()), Times.Once);


                MockInnerBuilder.Verify(x =>
                    x.Options.Configure(It.Is<UserAuthenticationTypeTransformerOptions>(o => o.AuthenticationType == "myAuthType")),
                    Times.Once);
            }
        }

        #endregion

        #region - WithTableauCloudUsernames -

        public class WithTableauCloudUsernames : ServerToCloudMigrationPlanBuilderTest
        {
            [Fact]
            public void WithMailDomainAndOverwrite()
            {
                Builder.WithTableauCloudUsernames("test.com", false);

                AssertRequiredUsernameExtensions("test.com", false);
            }

            [Fact]
            public void WithObject()
            {
                var myMapping = Create<Mock<ITableauCloudUsernameMapping>>().Object;

                Builder.WithTableauCloudUsernames(myMapping);

                MockInnerBuilder.Verify(x => x.Mappings.Add(myMapping), Times.Once);
            }

            private class TestUsernameMapping : ITableauCloudUsernameMapping
            {
                public Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
                    => ctx.ToTask();
            }

            [Fact]
            public void WithFactory()
            {
                var fact = (IServiceProvider services) => new TestUsernameMapping();

                Builder.WithTableauCloudUsernames<TestUsernameMapping>(fact);

                MockInnerBuilder.Verify(x => x.Mappings.Add<TestUsernameMapping, IUser>(fact), Times.Once);
            }

            [Fact]
            public void WithCallback()
            {
                var callback = (ContentMappingContext<IUser> ctx, CancellationToken cancel) =>
                    ctx.ToTask();

                Builder.WithTableauCloudUsernames(callback);

                MockInnerBuilder.Verify(x => x.Mappings.Add(callback), Times.Once);
            }
        }

        #endregion

        public class Validate : ServerToCloudMigrationPlanBuilderTest
        {
            [Fact]
            public void MissingAuthTypeMapping()
            {
                Builder.WithTableauCloudUsernames("salesforce.com");

                var result = Builder
                    .ValidateServerToCloud();

                result.AssertFailure();
                Assert.Single(result.Errors);
            }

            [Fact]
            public void MissingUsernameMapping()
            {
                Builder.WithTableauIdAuthenticationType();

                var result = Builder
                    .ValidateServerToCloud();

                result.AssertFailure();
                Assert.Single(result.Errors);
            }

            [Fact]
            public void AllRequiredInfo()
            {
                Builder.WithTableauCloudUsernames("salesforce.com")
                    .WithTableauIdAuthenticationType();

                var result = Builder.ValidateServerToCloud();

                result.AssertSuccess();
            }
        }
    }
}
