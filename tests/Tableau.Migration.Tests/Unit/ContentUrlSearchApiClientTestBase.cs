//
//  Copyright (c) 2026, Salesforce, Inc.
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

using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Tests.Unit.Api;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public abstract class ContentUrlSearchApiClientTestBase<TApiClient, TContent, TResponse> :
        FilteredPagedListApiClientTestBase<TApiClient, TContent, TResponse>
        where TApiClient : IContentUrlSearchApiClient<TContent>, IContentApiClient
        where TResponse : TableauServerResponse, new()
    {

        [Fact]
        public async Task SearchContentUrlAsync()
        {
            // Arrange
            var response = AutoFixture.CreateResponse<TResponse>();

            var mockResponse = new MockHttpResponseMessage<TResponse>(response);

            MockHttpClient.SetupResponse(mockResponse);

            // Act
            var results = await ApiClient.SearchByContentUrlAsync("testContentUrl", Cancel);

            // Assert

            results.AssertSuccess();

            var request = MockHttpClient.AssertSingleRequest();

            request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            request.AssertQuery("pageSize", "1");
            request.AssertQuery("pageNumber", "1");
            request.AssertQuery("filter", GetExpectedFilterExpression([new Filter("contentUrl", FilterOperator.Equal, "testContentUrl")]));
        }
    }
}
