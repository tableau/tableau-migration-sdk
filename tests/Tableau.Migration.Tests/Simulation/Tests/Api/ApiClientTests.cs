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
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class ApiClientTests
    {
        public abstract class ApiClientTest : ApiClientTestBase
        {
            public ApiClientTest(bool isCloud = false)
                : base(isCloud)
            { }

            protected void AssertDefaultRestApiVersion(string? expectedVersion)
            {
                var versionProvider = ServiceProvider.GetRequiredService<ITableauServerVersionProvider>();
                Assert.Equal(expectedVersion, versionProvider.Version?.RestApiVersion);
            }

            protected void AssertSession(Action<IServerSessionProvider> assert)
            {
                var sessionProvider = ServiceProvider.GetRequiredService<IServerSessionProvider>();
                assert(sessionProvider);
            }

            protected async Task AssertAuthenticationTokenAsync(Action<string?> assert)
            {
                var tokenProvider = ServiceProvider.GetRequiredService<IAuthenticationTokenProvider>();
                var token = await tokenProvider.GetAsync(Cancel);
                assert(token);
            }
        }

        public class GetServerInfoAsync : ApiClientTest
        {
            [Fact]
            public async Task Returns_server_info_on_success()
            {
                var restApiVersion = "9.99";
                var productVersion = "9999.9";
                var buildVersion = "99999.99.9999.9999";

                var serverInfo = new ServerInfoResponse.ServerInfoType(restApiVersion, productVersion, buildVersion);

                Api.Data.ServerInfo = serverInfo;

                var result = await ApiClient.GetServerInfoAsync(Cancel);

                Assert.True(result.Success);
                Assert.NotNull(result.Value);

                Assert.Equal(restApiVersion, result.Value.RestApiVersion);
                Assert.Equal(productVersion, result.Value.ProductVersion);
                Assert.Equal(buildVersion, result.Value.BuildVersion);

                AssertDefaultRestApiVersion(restApiVersion);
            }

            [Fact]
            public async Task Returns_error_on_failure()
            {
                Api.RestApi.QueryServerInfo.RespondWithError();

                var result = await ApiClient.GetServerInfoAsync(Cancel);

                Assert.False(result.Success);
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);

                AssertDefaultRestApiVersion(null);
            }
        }

        public class GetInstanceTypeForServerAsync : ApiClientTest
        {
            public GetInstanceTypeForServerAsync()
                : base(false)
            { }

            [Fact]
            public async Task Returns_Tableau_Server()
            {
                var result = await ApiClient.GetInstanceTypeAsync(Cancel);

                Assert.Equal(TableauInstanceType.Server, result);
            }

            [Fact]
            public async Task Returns_Unknown()
            {
                Api.RestApi.QuerySites.RespondWithError();

                var result = await ApiClient.GetInstanceTypeAsync(Cancel);

                Assert.Equal(TableauInstanceType.Unknown, result);
            }
        }

        public class GetInstanceTypeForCloudAsync : ApiClientTest
        {
            public GetInstanceTypeForCloudAsync()
                : base(true)
            { }

            [Fact]
            public async Task Returns_Tableau_Cloud()
            {
                var result = await ApiClient.GetInstanceTypeAsync(Cancel);

                Assert.Equal(TableauInstanceType.Cloud, result);
            }

            [Fact]
            public async Task Returns_Unknown()
            {
                Api.RestApi.QuerySites.RespondWithError();

                var result = await ApiClient.GetInstanceTypeAsync(Cancel);

                Assert.Equal(TableauInstanceType.Unknown, result);
            }
        }

        public class SignInAsync : ApiClientTest
        {
            [Fact]
            public async Task Returns_site_client_on_success()
            {
                var signIn = Create<SignInResponse.CredentialsType>();

                Assert.NotNull(signIn.User);
                Assert.NotNull(signIn.Site);

                Api.Data.SignIn = signIn;

                await using var result = await ApiClient.SignInAsync(Cancel);

                Assert.True(result.Success);
                Assert.NotNull(result.Value);

                AssertSession(p =>
                {
                    Assert.Equal(signIn.User.Id, p.UserId);
                    Assert.Equal(signIn.Site.Id, p.SiteId);
                    Assert.Equal(signIn.Site.ContentUrl, p.SiteContentUrl);
                });

                await AssertAuthenticationTokenAsync(token =>
                {
                    Assert.Equal(signIn.Token, token);
                });
            }

            [Fact]
            public async Task Returns_error_on_failure()
            {
                Api.RestApi.Auth.SignIn.RespondWithError();

                await using var result = await ApiClient.SignInAsync(Cancel);

                Assert.False(result.Success);
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);

                AssertSession(p =>
                {
                    Assert.Null(p.UserId);
                    Assert.Null(p.SiteId);
                    Assert.Null(p.SiteContentUrl);
                });

                await AssertAuthenticationTokenAsync(token =>
                {
                    Assert.Null(token);
                });
            }

            [Fact]
            public async Task Returns_error_on_invalid_credentials()
            {
                var errorBuilder = new StaticRestErrorBuilder(
                    HttpStatusCode.Unauthorized,
                    1,
                    "Login error",
                    "The credentials (name or password, or personal access token name or secret) are invalid for the specified site, or the site contentURL is invalid.");

                Api.RestApi.Auth.SignIn.RespondWithError(errorBuilder);

                await using var result = await ApiClient.SignInAsync(Cancel);

                Assert.False(result.Success);
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                var restException = Assert.IsType<RestException>(error);

                Assert.Equal("401001", restException.Code);
                Assert.Equal(errorBuilder.Summary, restException.Summary);
                Assert.Equal(errorBuilder.Detail, restException.Detail);

                AssertSession(p =>
                {
                    Assert.Null(p.UserId);
                    Assert.Null(p.SiteId);
                    Assert.Null(p.SiteContentUrl);
                });

                await AssertAuthenticationTokenAsync(token =>
                {
                    Assert.Null(token);
                });
            }
        }

        public class SignOutAsync : ApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                Assert.NotNull(sitesClient);

                var result = await sitesClient.SignOutAsync(Cancel);

                Assert.True(result.Success);

                await AssertAuthenticationTokenAsync(token =>
                {
                    Assert.Null(token);
                });
            }

            [Fact]
            public async Task Returns_error_on_failure()
            {
                Api.RestApi.Auth.SignOut.RespondWithError();

                await using var sitesClient = await GetSitesClientAsync(Cancel);

                Assert.NotNull(sitesClient);

                var result = await sitesClient.SignOutAsync(Cancel);

                Assert.False(result.Success);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);

                await AssertAuthenticationTokenAsync(token =>
                {
                    Assert.Null(token);
                });
            }
        }
    }
}
