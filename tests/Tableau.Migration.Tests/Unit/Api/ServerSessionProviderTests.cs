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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ServerSessionProviderTests
    {
        public abstract class ServerSessionProviderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IAuthenticationTokenProvider> MockTokenProvider = new();
            protected readonly Mock<ITableauServerVersionProvider> MockVersionProvider = new();

            internal readonly ServerSessionProvider Provider;

            public ServerSessionProviderTest()
            {
                Provider = new(MockVersionProvider.Object, MockTokenProvider.Object);
            }
        }

        public class GetAuthenticationTokenAsync : ServerSessionProviderTest
        {
            [Fact]
            public async Task GetsTokenAsync()
            {
                var token = Create<string>();

                MockTokenProvider.Setup(p => p.GetAsync(Cancel)).ReturnsAsync(token);

                Assert.Equal(token, await Provider.GetAuthenticationTokenAsync(Cancel));
            }
        }

        public class Version : ServerSessionProviderTest
        {
            [Fact]
            public void Returns_version()
            {
                var version = Create<TableauServerVersion>();

                MockVersionProvider.SetupGet(p => p.Version).Returns(version);

                Assert.Equal(version, Provider.Version);
            }
        }

        public class SetCurrentUserAndSiteAsync : ServerSessionProviderTest
        {
            [Fact]
            public async Task SetsUserAndSiteWithSignInResultAsync()
            {
                var signInResult = Create<ISignInResult>();

                await Provider.SetCurrentSessionAsync(signInResult, TableauInstanceType.Server, Cancel);

                Assert.Equal(signInResult.UserId, Provider.UserId);
                Assert.Equal(signInResult.SiteContentUrl, Provider.SiteContentUrl);
                Assert.Equal(signInResult.SiteId, Provider.SiteId);

                MockTokenProvider.Verify(p => p.SetAsync(signInResult.Token, Cancel), Times.Once);
            }

            [Fact]
            public async Task SetsUserAndSiteFromValuesAsync()
            {
                var userId = Create<Guid>();
                var siteContentUrl = Create<string>();
                var siteId = Create<Guid>();
                var token = Create<string>();

                await Provider.SetCurrentSessionAsync(userId, siteId, siteContentUrl, token, TableauInstanceType.Cloud, Cancel);

                Assert.Equal(userId, Provider.UserId);
                Assert.Equal(siteContentUrl, Provider.SiteContentUrl);
                Assert.Equal(siteId, Provider.SiteId);

                MockTokenProvider.Verify(p => p.SetAsync(token, Cancel), Times.Once);
            }
        }

        public class ClearCurrentUserAndSite : ServerSessionProviderTest
        {
            [Fact]
            public async Task ClearsUserAsync()
            {
                var signInResult = Create<ISignInResult>();

                await Provider.SetCurrentSessionAsync(signInResult, TableauInstanceType.Server, Cancel);

                await Provider.ClearCurrentSessionAsync(Cancel);

                Assert.Null(Provider.UserId);
                Assert.Null(Provider.SiteContentUrl);
                Assert.Null(Provider.SiteId);
                Assert.Equal(TableauInstanceType.Unknown, Provider.InstanceType);

                MockTokenProvider.Verify(p => p.ClearAsync(Cancel), Times.Once);
            }
        }

        public class SetVersion : ServerSessionProviderTest
        {
            [Fact]
            public void Sets_version()
            {
                var version = Create<TableauServerVersion>();

                Provider.SetVersion(version);

                MockVersionProvider.Verify(p => p.Set(version), Times.Once);
            }
        }
    }
}
