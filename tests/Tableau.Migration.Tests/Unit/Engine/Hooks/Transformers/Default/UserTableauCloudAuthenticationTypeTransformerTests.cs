//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class UserTableauCloudAuthenticationTypeTransformerTests
    {
        public class ExecuteAsync : OptionsHookTestBase<UserAuthenticationTypeTransformerOptions>
        {
            private readonly Mock<IDestinationAuthenticationConfigurationsCache> MockDestinationCache;
            
            private List<IAuthenticationConfiguration> AuthenticationConfigurations { get; set; }

            public ExecuteAsync()
            {
                AuthenticationConfigurations = new();

                MockDestinationCache = Freeze<Mock<IDestinationAuthenticationConfigurationsCache>>();
                MockDestinationCache.Setup(x => x.GetAllAsync(Cancel))
                    .ReturnsAsync(() => AuthenticationConfigurations.ToImmutableArray());
            }

            [Fact]
            public async Task SetsServerDefaultAuthType()
            {
                AuthenticationConfigurations.Clear();

                var t = Create<UserAuthenticationTypeTransformer>();

                var mockUser = new Mock<IUser>();
                var resultUser = await t.ExecuteAsync(mockUser.Object, Cancel);

                Assert.Same(resultUser, mockUser.Object);
                mockUser.VerifySet(x => x.Authentication = UserAuthenticationType.ForAuthenticationType(AuthenticationTypes.ServerDefault), Times.Once);

                MockDestinationCache.Verify(x => x.GetAllAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task SetsAuthSettingAsync()
            {
                AuthenticationConfigurations.Clear();
                Options = new UserAuthenticationTypeTransformerOptions
                {
                    AuthenticationType = AuthenticationTypes.TableauIdWithMfa
                };

                var t = Create<UserAuthenticationTypeTransformer>();

                var mockUser = new Mock<IUser>();
                var resultUser = await t.ExecuteAsync(mockUser.Object, Cancel);

                Assert.Same(resultUser, mockUser.Object);
                mockUser.VerifySet(x => x.Authentication = UserAuthenticationType.ForAuthenticationType(AuthenticationTypes.TableauIdWithMfa), Times.Once);

                MockDestinationCache.Verify(x => x.GetAllAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task MatchesIdpConfigurationNameAsync()
            {
                AuthenticationConfigurations = CreateMany<IAuthenticationConfiguration>().ToList();
                var authType = AuthenticationConfigurations.PickRandom();

                Options = new UserAuthenticationTypeTransformerOptions
                {
                    AuthenticationType = authType.IdpConfigurationName
                };

                var t = Create<UserAuthenticationTypeTransformer>();

                var mockUser = new Mock<IUser>();
                var resultUser = await t.ExecuteAsync(mockUser.Object, Cancel);

                Assert.Same(resultUser, mockUser.Object);
                mockUser.VerifySet(x => x.Authentication = UserAuthenticationType.ForConfigurationId(authType.Id), Times.Once);

                MockDestinationCache.Verify(x => x.GetAllAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task MatchesAuthSettingAsync()
            {
                AuthenticationConfigurations = CreateMany<IAuthenticationConfiguration>().ToList();
                var authType = AuthenticationConfigurations.PickRandom();

                Options = new UserAuthenticationTypeTransformerOptions
                {
                    AuthenticationType = authType.AuthSetting
                };

                var t = Create<UserAuthenticationTypeTransformer>();

                var mockUser = new Mock<IUser>();
                var resultUser = await t.ExecuteAsync(mockUser.Object, Cancel);

                Assert.Same(resultUser, mockUser.Object);
                mockUser.VerifySet(x => x.Authentication = UserAuthenticationType.ForConfigurationId(authType.Id), Times.Once);

                MockDestinationCache.Verify(x => x.GetAllAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task MultipleIdpConfigurationNamesAsync()
            {
                AuthenticationConfigurations = CreateMany<IAuthenticationConfiguration>().ToList();
                var authType = AuthenticationConfigurations.PickRandom();

                AuthenticationConfigurations.Add(authType);
                Options = new UserAuthenticationTypeTransformerOptions
                {
                    AuthenticationType = authType.IdpConfigurationName
                };

                var t = Create<UserAuthenticationTypeTransformer>();

                var mockUser = new Mock<IUser>();
                await Assert.ThrowsAsync<ArgumentException>(() => t.ExecuteAsync(mockUser.Object, Cancel));

                mockUser.VerifySet(x => x.Authentication = It.IsAny<UserAuthenticationType>(), Times.Never);

                MockDestinationCache.Verify(x => x.GetAllAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task MultipleAuthSettingsAsync()
            {
                AuthenticationConfigurations = CreateMany<IAuthenticationConfiguration>().ToList();
                var authType = AuthenticationConfigurations.PickRandom();

                AuthenticationConfigurations.Add(authType);
                Options = new UserAuthenticationTypeTransformerOptions
                {
                    AuthenticationType = authType.AuthSetting
                };

                var t = Create<UserAuthenticationTypeTransformer>();

                var mockUser = new Mock<IUser>();
                await Assert.ThrowsAsync<ArgumentException>(() => t.ExecuteAsync(mockUser.Object, Cancel));

                mockUser.VerifySet(x => x.Authentication = It.IsAny<UserAuthenticationType>(), Times.Never);

                MockDestinationCache.Verify(x => x.GetAllAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task NoMatchAsync()
            {
                AuthenticationConfigurations = CreateMany<IAuthenticationConfiguration>().ToList();

                Options = new UserAuthenticationTypeTransformerOptions
                {
                    AuthenticationType = Guid.NewGuid().ToString()
                };

                var t = Create<UserAuthenticationTypeTransformer>();

                var mockUser = new Mock<IUser>();
                await Assert.ThrowsAsync<ArgumentException>(() => t.ExecuteAsync(mockUser.Object, Cancel));

                mockUser.VerifySet(x => x.Authentication = It.IsAny<UserAuthenticationType>(), Times.Never);

                MockDestinationCache.Verify(x => x.GetAllAsync(Cancel), Times.Once);
            }
        }
    }
}
