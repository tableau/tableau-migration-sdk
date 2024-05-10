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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
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
            protected readonly Mock<IPagedListApiClient<IUser>> MockApiClient;
            protected readonly Mock<IReadApiClient<IUser>> MockReadApiClient;

            private static int BatchSize = 10;

            protected ImmutableArray<IUser> Data { get; set; }

            protected readonly BulkApiContentReferenceCache<IUser> Cache;

            public SearchAsync()
            {
                MockApiClient = new Mock<IPagedListApiClient<IUser>>
                {
                    CallBase = true
                };
                MockApiClient.Setup(x => x.GetPager(BatchSize))
                    .Returns((int pageSize) => new MemoryPager<IUser>(Data, pageSize));
                MockReadApiClient = new Mock<IReadApiClient<IUser>>
                {
                    CallBase = true
                };

                Data = CreateMany<IUser>()
                    .ToImmutableArray();

                var mockConfigReader = Freeze<Mock<IConfigReader>>();
                mockConfigReader.Setup(x => x.Get<TestContentType>())
                    .Returns(() => new ContentTypesOptions() { BatchSize = BatchSize });

                mockConfigReader.Setup(x => x.Get<IUser>())
                    .Returns(() => new ContentTypesOptions() { BatchSize = BatchSize });

                var mockSitesApi = Freeze<Mock<ISitesApiClient>>();
                mockSitesApi.Setup(x => x.GetListApiClient<IUser>())
                    .Returns(MockApiClient.Object);
                mockSitesApi.Setup(x => x.GetReadApiClient<IUser>())
                    .Returns(MockReadApiClient.Object);

                Cache = Create<BulkApiContentReferenceCache<IUser>>();
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
            public async Task NotFoundByLocationAsync()
            {
                var search = Create<IUser>();

                var result = await Cache.ForLocationAsync(search.Location, Cancel);

                Assert.Null(result);
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
            public async Task NotFoundByIdAsync()
            {
                var search = Create<IUser>();

                var result = await Cache.ForIdAsync(search.Id, Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task NotFoundByIdWithFallbackAsync()
            {
                var search = Create<IUser>();

                MockReadApiClient.Setup(x => x.GetByIdAsync(search.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IUser>.Succeeded(search));

                var result = await Cache.ForIdAsync(search.Id, Cancel);

                Assert.NotNull(result);
                var resultStub = Assert.IsType<ContentReferenceStub>(result);
                Assert.Equal(new ContentReferenceStub(search), resultStub);
            }

            [Fact]
            public async Task FailureByLocationReturnsEmptyAsync()
            {
                var mockFailurePager = Create<Mock<IPager<IUser>>>();
                mockFailurePager.Setup(x => x.NextPageAsync(Cancel))
                    .ReturnsAsync(PagedResult<IUser>.Failed(new Exception()));

                MockApiClient.Setup(x => x.GetPager(BatchSize))
                    .Returns(mockFailurePager.Object);

                var result = await Cache.ForLocationAsync(new(), Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task FailureByIdReturnsEmptyAsync()
            {
                var mockFailurePager = Create<Mock<IPager<IUser>>>();
                mockFailurePager.Setup(x => x.NextPageAsync(Cancel))
                    .ReturnsAsync(PagedResult<IUser>.Failed(new Exception()));

                MockApiClient.Setup(x => x.GetPager(BatchSize))
                    .Returns(mockFailurePager.Object);

                var result = await Cache.ForIdAsync(Guid.NewGuid(), Cancel);

                Assert.Null(result);
            }
        }
    }
}
