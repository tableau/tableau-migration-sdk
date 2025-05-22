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
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public sealed class BulkApiAuthenticationConfigurationsCacheTests
    {
        public sealed class GetAllAsync : AutoFixtureTestBase
        {
            private readonly Mock<IAuthenticationConfigurationsApiClient> _mockApiClient;
            private readonly BulkApiAuthenticationConfigurationsCache _cache;

            private List<IAuthenticationConfiguration> AuthenticationConfigurations { get; set; }

            public GetAllAsync()
            {
                AuthenticationConfigurations = CreateMany<IAuthenticationConfiguration>().ToList();

                _mockApiClient = Freeze<Mock<IAuthenticationConfigurationsApiClient>>();
                _mockApiClient.Setup(x => x.GetAllAsync(AuthenticationConfigurationsApiClient.MAX_CONFIGURATIONS, Cancel))
                    .ReturnsAsync(() => Result<IImmutableList<IAuthenticationConfiguration>>.Succeeded(AuthenticationConfigurations.ToImmutableArray()));

                var mockEndpoint = Create<Mock<IDestinationApiEndpoint>>();
                mockEndpoint.SetupGet(x => x.SiteApi.AuthenticationConfigurations).Returns(_mockApiClient.Object);

                _cache = new BulkApiAuthenticationConfigurationsCache(mockEndpoint.Object);
            }

            [Fact]
            public async Task LoadsAndGetsCacheAsync()
            {
                var configs = await _cache.GetAllAsync(Cancel);
                Assert.Equal(AuthenticationConfigurations, configs);

                configs = await _cache.GetAllAsync(Cancel);
                Assert.Equal(AuthenticationConfigurations, configs);

                _mockApiClient.Verify(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task LoadFailureAsync()
            {
                _mockApiClient.Setup(x => x.GetAllAsync(AuthenticationConfigurationsApiClient.MAX_CONFIGURATIONS, Cancel))
                    .ReturnsAsync(() => Result<IImmutableList<IAuthenticationConfiguration>>.Failed(new Exception()));

                var configs = await _cache.GetAllAsync(Cancel);
                Assert.Empty(configs);

                _mockApiClient.Verify(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
