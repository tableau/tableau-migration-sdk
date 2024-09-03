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

using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class CustomViewsApiClientTests
    {
        public class CustomViewsApiClientTest : ApiClientTestBase<ICustomViewsApiClient, CustomViewsResponse.CustomViewResponseType>
        { }

        #region - GetAllCustomViewsAsync -

        public class GetAllCustomViewsAsync : CustomViewsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                Api.Data.CreateCustomViews(AutoFixture, 11);

                // Act
                var result = await sitesClient.CustomViews.GetAllCustomViewsAsync(1, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                // Act
                result = await sitesClient.CustomViews.GetAllCustomViewsAsync(2, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                // Act
                result = await sitesClient.CustomViews.GetAllCustomViewsAsync(3, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Single(result.Value);
            }
        }

        #endregion

        #region - DownloadCustomViewAsync -

        public class DownloadCustomViewAsync : CustomViewsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var customView = Api.Data.CreateCustomView(AutoFixture);

                Assert.NotNull(customView);

                Api.Data.AddCustomView(customView, fileData: Constants.DefaultEncoding.GetBytes(customView.ToJson()));

                var result = await sitesClient.CustomViews.DownloadCustomViewAsync(customView.Id, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        #endregion
    }
}
