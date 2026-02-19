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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Paging;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net.Rest.Filtering;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public abstract class FilteredPagedListApiClientTestBase<TApiClient, TContent, TResponse> : 
        PagedListApiClientTestBase<TApiClient, TContent, TResponse>
        where TApiClient : IFilteredPagedListApiClient<TContent>, IContentApiClient
        where TResponse : TableauServerResponse, new()
    {
        protected virtual IEnumerable<Filter> TestFilters { get; } = [new Filter("test", FilterOperator.Equal, "foo")];

        protected virtual string GetExpectedFilterExpression(IEnumerable<Filter> testFilters)
        {
            var filters = testFilters.Select(f => f.Expression);
            return string.Join(",", filters);
        }

        [Fact]
        public async Task GetFilteredPagerAsync()
        {
            // Arrange
            var response = AutoFixture.CreateResponse<TResponse>();

            var mockResponse = new MockHttpResponseMessage<TResponse>(response);

            MockHttpClient.SetupResponse(mockResponse);

            // Act
            var pager = ApiClient.GetPager(TestFilters, 123);
            var pageResult = await pager.NextPageAsync(Cancel);

            // Assert

            Assert.IsType<ApiFilteredListPager<TContent>>(pager);
            pageResult.AssertSuccess();

            var request = MockHttpClient.AssertSingleRequest();

            request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            request.AssertQuery("pageSize", "123");
            request.AssertQuery("pageNumber", "1");
            request.AssertQuery("filter", GetExpectedFilterExpression(TestFilters));
        }
    }
}
