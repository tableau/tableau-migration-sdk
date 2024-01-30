// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Search
{
    public class BulkApiContentReferenceCacheTests
    {
        public class SearchAsync : AutoFixtureTestBase
        {
            protected readonly Mock<IPagedListApiClient<TestContentType>> MockApiClient;

            protected int BatchSize { get; set; } = 10;

            protected ImmutableArray<TestContentType> Data { get; set; }

            protected readonly BulkApiContentReferenceCache<TestContentType> Cache;

            public SearchAsync()
            {
                MockApiClient = new Mock<IPagedListApiClient<TestContentType>>
                {
                    CallBase = true
                };
                MockApiClient.Setup(x => x.GetPager(BatchSize))
                    .Returns((int pageSize) => new MemoryPager<TestContentType>(Data, pageSize));

                Data = CreateMany<TestContentType>()
                    .ToImmutableArray();

                var mockConfigReader = Freeze<Mock<IConfigReader>>();
                mockConfigReader.Setup(x => x.Get())
                    .Returns(() => new MigrationSdkOptions { BatchSize = BatchSize });

                var mockSitesApi = Freeze<Mock<ISitesApiClient>>();
                mockSitesApi.Setup(x => x.GetListApiClient<TestContentType>())
                    .Returns(MockApiClient.Object);

                Cache = Create<BulkApiContentReferenceCache<TestContentType>>();
            }

            [Fact]
            public async Task SuccessByLocationAsync()
            {
                var search = Data.First();

                var result = await Cache.ForLocationAsync(search.Location, Cancel);

                Assert.NotNull(result);
                var resultStub = Assert.IsType<ContentReferenceStub>(result);
                Assert.Equal(new ContentReferenceStub(search), resultStub);
            }

            [Fact]
            public async Task SuccessByIdAsync()
            {
                var search = Data.First();

                var result = await Cache.ForIdAsync(search.Id, Cancel);

                Assert.NotNull(result);
                var resultStub = Assert.IsType<ContentReferenceStub>(result);
                Assert.Equal(new ContentReferenceStub(search), resultStub);
            }

            [Fact]
            public async Task FailureByLocationReturnsEmptyAsync()
            {
                var mockFailurePager = Create<Mock<IPager<TestContentType>>>();
                mockFailurePager.Setup(x => x.NextPageAsync(Cancel))
                    .ReturnsAsync(PagedResult<TestContentType>.Failed(new Exception()));

                MockApiClient.Setup(x => x.GetPager(BatchSize))
                    .Returns(mockFailurePager.Object);

                var result = await Cache.ForLocationAsync(new(), Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task FailureByIdReturnsEmptyAsync()
            {
                var mockFailurePager = Create<Mock<IPager<TestContentType>>>();
                mockFailurePager.Setup(x => x.NextPageAsync(Cancel))
                    .ReturnsAsync(PagedResult<TestContentType>.Failed(new Exception()));

                MockApiClient.Setup(x => x.GetPager(BatchSize))
                    .Returns(mockFailurePager.Object);

                var result = await Cache.ForIdAsync(Guid.NewGuid(), Cancel);

                Assert.Null(result);
            }
        }
    }
}
