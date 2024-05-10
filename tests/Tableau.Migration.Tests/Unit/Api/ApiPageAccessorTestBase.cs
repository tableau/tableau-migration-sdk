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
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public abstract class ApiPageAccessorTestBase<TApiClient, TContent, TResponse> : ApiClientTestBase<TApiClient>
        where TApiClient : IApiPageAccessor<TContent>
        where TResponse : TableauServerResponse, new()
    {
        [Fact]
        public async Task GetPage_success()
        {
            // Arrange
            var response = AutoFixture.CreateResponse<TResponse>();

            var mockResponse = new MockHttpResponseMessage<TResponse>(response);

            MockHttpClient.SetupResponse(mockResponse);

            // Act
            var result = await ApiClient.GetPageAsync(2, 15, Cancel);

            // Assert
            result.AssertSuccess();

            var request = MockHttpClient.AssertSingleRequest();

            request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            request.AssertQuery("pageSize", "15");
            request.AssertQuery("pageNumber", "2");
        }

        [Fact]
        public async Task GetPage_failure()
        {
            // Arrange
            var exception = new Exception();

            var mockResponse = new MockHttpResponseMessage<TResponse>(HttpStatusCode.InternalServerError, null);
            mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

            MockHttpClient.SetupResponse(mockResponse);

            // Act
            var result = await ApiClient.GetPageAsync(3, 47, Cancel);

            // Assert
            result.AssertFailure();

            var request = MockHttpClient.AssertSingleRequest();

            request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            request.AssertQuery("pageSize", "47");
            request.AssertQuery("pageNumber", "3");
        }
    }
}
