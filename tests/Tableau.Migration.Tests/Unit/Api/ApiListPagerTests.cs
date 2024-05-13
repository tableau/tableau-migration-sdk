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

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ApiListPagerTests
    {
        public class NextPageAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task ListsPagesInSequenceAsync()
            {
                var pageSize = 47;
                var cancel = new CancellationToken();

                var apiListPages = CreateMany<IPagedResult<TestContentType>>(3).ToImmutableArray();

                var mockListApi = Create<Mock<IApiPageAccessor<TestContentType>>>();
                mockListApi.Setup(x => x.GetPageAsync(It.IsAny<int>(), pageSize, cancel))
                    .ReturnsAsync((int pageNumber, int pageSize, CancellationToken c) => apiListPages[pageNumber - 1]);

                var pager = new ApiListPager<TestContentType>(mockListApi.Object, pageSize);

                var result1 = await pager.NextPageAsync(cancel);
                Assert.Same(apiListPages[0], result1);
                mockListApi.Verify(x => x.GetPageAsync(1, pageSize, cancel), Times.Once);

                var result2 = await pager.NextPageAsync(cancel);
                Assert.Same(apiListPages[1], result2);
                mockListApi.Verify(x => x.GetPageAsync(2, pageSize, cancel), Times.Once);

                var result3 = await pager.NextPageAsync(cancel);
                Assert.Same(apiListPages[2], result3);
                mockListApi.Verify(x => x.GetPageAsync(3, pageSize, cancel), Times.Once);

                mockListApi.Verify(x => x.GetPageAsync(It.IsAny<int>(), pageSize, cancel), Times.Exactly(3));
            }
        }
    }
}
