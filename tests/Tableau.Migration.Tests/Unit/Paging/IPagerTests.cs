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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public class IPagerTests
    {
        public class GetAllPagesAsync : AutoFixtureTestBase
        {
            private readonly Queue<IPagedResult<TestContentType>> _pages;

            private readonly Mock<IPager<TestContentType>> MockPager;

            public GetAllPagesAsync()
            {
                _pages = new();

                MockPager = new Mock<IPager<TestContentType>> { CallBase = true };
                MockPager.Setup(x => x.NextPageAsync(Cancel)).ReturnsAsync(() => _pages.Dequeue());
            }

            private void EnqueuePage(IPagedResult<TestContentType> result) => _pages.Enqueue(result);

            private void EnqueuePage(int pageSize, int totalCount, IEnumerable<TestContentType> items, bool lastPage)
                => EnqueuePage(PagedResult<TestContentType>.Succeeded(items.ToImmutableArray(), _pages.Count + 1, pageSize, totalCount, lastPage));

            [Fact]
            public async Task GetsAllItemsUntilTotalCountAsync()
            {
                var items = CreateMany<TestContentType>(10);

                EnqueuePage(2, 10, items.Skip(0).Take(2), false);
                EnqueuePage(2, 10, items.Skip(2).Take(2), false);
                EnqueuePage(2, 10, items.Skip(4).Take(2), false);
                EnqueuePage(2, 10, items.Skip(6).Take(2), false);
                EnqueuePage(2, 10, items.Skip(8).Take(2), true);

                var results = await MockPager.Object.GetAllPagesAsync(Cancel);

                results.AssertSuccess();
                Assert.NotNull(results.Value);
                Assert.Equal(items, results.Value!);

                MockPager.Verify(x => x.NextPageAsync(Cancel), Times.Exactly(5));
            }

            [Fact]
            public async Task GetsAllItemsWithUnexpectedEmptyPageAsync()
            {
                var items = CreateMany<TestContentType>(10);

                EnqueuePage(2, 10, items.Skip(0).Take(2), false);
                EnqueuePage(2, 10, items.Skip(2).Take(2), false);
                EnqueuePage(2, 10, items.Skip(4).Take(2), false);
                EnqueuePage(2, 10, Enumerable.Empty<TestContentType>(), false);
                EnqueuePage(2, 10, items.Skip(6).Take(2), true);

                var results = await MockPager.Object.GetAllPagesAsync(Cancel);

                results.AssertSuccess();
                Assert.NotNull(results.Value);
                Assert.Equal(items.Take(8), results.Value!);

                MockPager.Verify(x => x.NextPageAsync(Cancel), Times.Exactly(5));
            }

            [Fact]
            public async Task GetsAllItemsUntilFailureAsync()
            {
                var items = CreateMany<TestContentType>(10);

                EnqueuePage(2, 10, items.Skip(0).Take(2), false);
                EnqueuePage(2, 10, items.Skip(2).Take(2), false);
                EnqueuePage(2, 10, items.Skip(4).Take(2), false);
                EnqueuePage(PagedResult<TestContentType>.Failed(new Exception()));
                EnqueuePage(2, 10, items.Skip(8).Take(2), false);

                var results = await MockPager.Object.GetAllPagesAsync(Cancel);

                results.AssertFailure();
                Assert.NotNull(results.Value);
                Assert.Equal(items.Take(6), results.Value!);

                MockPager.Verify(x => x.NextPageAsync(Cancel), Times.Exactly(4));
            }

            [Fact]
            public async Task GetsAllItemsLastPageIncomplete()
            {
                var items = CreateMany<TestContentType>(10);

                EnqueuePage(2, 10, items.Skip(0).Take(2), false);
                EnqueuePage(2, 10, items.Skip(2).Take(2), false);
                EnqueuePage(2, 10, items.Skip(4).Take(2), false);
                EnqueuePage(2, 10, items.Skip(6).Take(2), false);
                EnqueuePage(2, 10, items.Skip(8).Take(1), true);
                EnqueuePage(PagedResult<TestContentType>.Failed(new Exception()));

                var results = await MockPager.Object.GetAllPagesAsync(Cancel);

                results.AssertSuccess();
                Assert.NotNull(results.Value);
                Assert.Equal(items.Take(9), results.Value!);

                MockPager.Verify(x => x.NextPageAsync(Cancel), Times.Exactly(5));
            }

            [Fact]
            public async Task GetsAllItemsTwoPagesIncomplete()
            {
                var items = CreateMany<TestContentType>(10);

                EnqueuePage(2, 10, items.Skip(0).Take(2), false);
                EnqueuePage(2, 10, items.Skip(2).Take(2), false);
                EnqueuePage(2, 10, items.Skip(4).Take(1), false);
                EnqueuePage(2, 10, items.Skip(5).Take(2), false);
                EnqueuePage(2, 10, items.Skip(7).Take(1), true);
                EnqueuePage(PagedResult<TestContentType>.Failed(new Exception()));

                var results = await MockPager.Object.GetAllPagesAsync(Cancel);

                results.AssertSuccess();
                Assert.NotNull(results.Value);
                Assert.Equal(items.Take(8), results.Value!);

                MockPager.Verify(x => x.NextPageAsync(Cancel), Times.Exactly(5));
            }
        }
    }
}
