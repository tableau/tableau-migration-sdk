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
using System.Net.Http;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class FlowsApiClientTests
    {
        public abstract class FlowsApiClientTest : ApiClientTestBase<IFlowsApiClient>
        {
            internal FlowsApiClient FlowsApiClient => GetApiClient<FlowsApiClient>();
        }

        #region - List -

        public class ListClient : PagedListApiClientTestBase<IFlowsApiClient, IFlow, FlowsResponse>
        { }

        public class PageAccessor : ApiPageAccessorTestBase<IFlowsApiClient, IFlow, FlowsResponse>
        { }

        #endregion

        #region - Download -

        public class DownloadDataSourceAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ApiClient.DownloadFlowAsync(flowId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/flows/{flowId}/content");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ApiClient.DownloadFlowAsync(flowId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/flows/{flowId}/content");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var content = new ByteArrayContent(Constants.DefaultEncoding.GetBytes("hi2u"));

                var mockResponse = new MockHttpResponseMessage(content);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ApiClient.DownloadFlowAsync(flowId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/flows/{flowId}/content");
            }
        }

        #endregion
    }
}
