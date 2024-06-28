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
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class SitesApiClientTests
    {
        public class SitesApiClientTest : ApiClientTestBase
        {
            public SitesApiClientTest(bool isCloud = false)
                : base(isCloud)
            { }
        }

        public class GetSiteAsync : SitesApiClientTest
        {
            [Fact]
            public async Task SuccessWithIdAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var site = Create<SiteResponse.SiteType>();
                Api.Data.Sites.Add(site);

                // Act
                var result = await sitesClient.GetSiteAsync(site.Id, Cancel);

                // Assert
                result.AssertSuccess();
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task NotFoundWithIdAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.GetSiteAsync(Guid.NewGuid(), Cancel);

                // Assert
                result.AssertFailure();
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);
            }

            [Fact]
            public async Task SuccessWithContentUrlAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var site = Create<SiteResponse.SiteType>();
                Api.Data.Sites.Add(site);

                // Act
                var result = await sitesClient.GetSiteAsync(site.ContentUrl!, Cancel);

                // Assert
                result.AssertSuccess();
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task NotFoundWithContentUrlAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.GetSiteAsync("myContentUrl", Cancel);

                // Assert
                result.AssertFailure();
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);
            }
        }

        public class GetTasksForServer : SitesApiClientTest
        {
            public GetTasksForServer()
                : base(false)
            { }

            [Fact]
            public async Task GetServerTasks_Returns_ServerTasksApiClient()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act/Assert
                Assert.IsAssignableFrom<IServerTasksApiClient>(sitesClient.ServerTasks);
            }

            [Fact]
            public async Task GetCloudTasks_Throws_Exception()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act/Assert
                Assert.Throws<TableauInstanceTypeNotSupportedException>(() => sitesClient.CloudTasks);
            }
        }

        public class GetTasksForCloud : SitesApiClientTest
        {
            public GetTasksForCloud()
                : base(true)
            { }

            [Fact]
            public async Task GetServerTasks_Throws_Exception()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act/Assert
                Assert.Throws<TableauInstanceTypeNotSupportedException>(() => sitesClient.ServerTasks);
            }

            [Fact]
            public async Task GetCloudTasks_Returns_CloudTasksApiClient()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act/Assert
                Assert.IsAssignableFrom<IServerTasksApiClient>(sitesClient.CloudTasks);
            }
        }
    }
}
