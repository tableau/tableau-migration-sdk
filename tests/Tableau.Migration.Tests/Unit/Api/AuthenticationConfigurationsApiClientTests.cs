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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public sealed class AuthenticationConfigurationsApiClientTests
    {
        public abstract class AuthenticationConfigurationsApiClientTest : SiteApiTestBase
        {
            internal readonly AuthenticationConfigurationsApiClient ApiClient;

            public AuthenticationConfigurationsApiClientTest()
            {
                InstanceType = TableauInstanceType.Cloud;
                ApiClient = (AuthenticationConfigurationsApiClient)Dependencies.CreateClient<IAuthenticationConfigurationsApiClient>();
            }
        }

        #region - GetAuthenticationConfigurationsAsync -

        public sealed class GetAuthenticationConfigurationsAsync : AuthenticationConfigurationsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<SiteAuthConfigurationsResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetAuthenticationConfigurationsAsync(Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/site-auth-configurations");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<SiteAuthConfigurationsResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetAuthenticationConfigurationsAsync(Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/site-auth-configurations");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var configResponse = AutoFixture.CreateResponse<SiteAuthConfigurationsResponse>();

                var mockResponse = new MockHttpResponseMessage<SiteAuthConfigurationsResponse>(configResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetAuthenticationConfigurationsAsync(Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/site-auth-configurations");
            }

            [Fact]
            public async Task ServerNotSupportedAsync()
            {
                InstanceType = TableauInstanceType.Server;

                var result = await ApiClient.GetAuthenticationConfigurationsAsync(Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Empty(result.Value);

                MockHttpClient.AssertNoRequests();
            }
        }

        #endregion

        #region - GetPager -

        public sealed class GetPager : AuthenticationConfigurationsApiClientTest
        {
            [Fact]
            public async Task GetsPagerAsync()
            {
                var configResponse = AutoFixture.CreateResponse<SiteAuthConfigurationsResponse>();

                var mockResponse = new MockHttpResponseMessage<SiteAuthConfigurationsResponse>(configResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var pager = ApiClient.GetPager(AuthenticationConfigurationsApiClient.MAX_CONFIGURATIONS);

                var result = await pager.GetAllPagesAsync(Cancel);

                result.AssertSuccess();
                Assert.Equal(configResponse.Items.Count(), result.Value!.Count);
            }

            [Fact]
            public async Task EmptyOnFailureAsync()
            {
                var mockResponse = new MockHttpResponseMessage<SiteAuthConfigurationsResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var pager = ApiClient.GetPager(AuthenticationConfigurationsApiClient.MAX_CONFIGURATIONS);

                var result = await pager.GetAllPagesAsync(Cancel);

                result.AssertFailure();
                Assert.Empty(result.Value!);
            }
        }

        #endregion
    }
}
