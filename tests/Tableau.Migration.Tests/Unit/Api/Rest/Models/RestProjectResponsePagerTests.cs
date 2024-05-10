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

using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class RestProjectResponsePagerTests
    {
        public class GetPageAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task UsesApiClientAsync()
            {
                var cancel = new CancellationToken();
                var expectedResult = Freeze<IPagedResult<ProjectsResponse.ProjectType>>();
                var mockApiClient = Create<Mock<IProjectsResponseApiClient>>();

                var pager = new RestProjectResponsePager(mockApiClient.Object, 123);

                var result = await pager.NextPageAsync(cancel);

                Assert.Same(expectedResult, result);
                mockApiClient.Verify(x => x.GetAllProjectsAsync(1, 123, cancel), Times.Once());
            }
        }
    }
}
