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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Search
{
    public class ContentReferenceCacheBaseTests
    {
        #region - Test Classes -

        public class TestContentReferenceCache : ContentReferenceCacheBase
        {
            public IEnumerable<ContentReferenceStub> SearchData { get; set; }
                = Enumerable.Empty<ContentReferenceStub>();

            public TimeSpan? SearchSpinTime { get; set; }

            public Exception? SearchException { get; set; }

            public int SearchCalls { get; private set; }

            protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(ContentLocation searchLocation, CancellationToken cancel)
            {
                SearchCalls++;
                if (SearchSpinTime is not null)
                {
                    await Task.Delay(SearchSpinTime.Value, cancel);
                }

                if (SearchException is not null)
                {
                    throw SearchException;
                }

                return SearchData;
            }

            protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(Guid searchId, CancellationToken cancel)
            {
                SearchCalls++;
                if (SearchSpinTime is not null)
                {
                    await Task.Delay(SearchSpinTime.Value, cancel);
                }

                if (SearchException is not null)
                {
                    throw SearchException;
                }

                return SearchData;
            }
        }

        public class ContentReferenceCacheBaseTest : AutoFixtureTestBase
        {
            protected readonly TestContentReferenceCache Cache;

            public ContentReferenceCacheBaseTest()
            {
                Cache = new();
            }
        }

        #endregion

        #region - ForLocationAsync -

        public class ForLocationAsync : ContentReferenceCacheBaseTest
        {
            [Fact]
            public async Task CachesByLocationAsync()
            {
                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.SearchData.First();

                var result = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Same(searchItem, result);

                //Test value cached.
                result = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Same(searchItem, result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task CachesByIdAsync()
            {
                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.SearchData.First();

                var result = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Same(searchItem, result);

                //Test value cached.
                result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Same(searchItem, result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task MultipleReturnValuesCachedAsync()
            {
                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>(),
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.SearchData.First();
                var extraItem = Cache.SearchData.Last();

                var result = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Same(searchItem, result);

                //Test value cached.
                result = await Cache.ForLocationAsync(extraItem.Location, Cancel);
                Assert.Same(extraItem, result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task NotFoundCachedAsync()
            {
                var notFoundItem = Create<ContentReferenceStub>();

                var result = await Cache.ForLocationAsync(notFoundItem.Location, Cancel);
                Assert.Null(result);

                result = await Cache.ForLocationAsync(notFoundItem.Location, Cancel);
                Assert.Null(result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task NotFoundSingleListAsync()
            {
                var notFoundItem1 = Create<ContentReferenceStub>();
                var notFoundItem2 = Create<ContentReferenceStub>();

                var result = await Cache.ForLocationAsync(notFoundItem1.Location, Cancel);
                Assert.Null(result);

                result = await Cache.ForLocationAsync(notFoundItem1.Location, Cancel);
                Assert.Null(result);

                Assert.Equal(1, Cache.SearchCalls);

                result = await Cache.ForLocationAsync(notFoundItem2.Location, Cancel);
                Assert.Null(result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            // TODO: W-14187810 - Fix Flaky Test.
            // Increasing the timeout configuration helps when the test runs in a machine with limited resources.
            [Fact]
            public async Task ThreadSafePopulationAsync()
            {
                Cache.SearchSpinTime = TimeSpan.FromMilliseconds(500);

                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.SearchData.First();

                var tasks = new[]
                {
                    Cache.ForLocationAsync(searchItem.Location, Cancel),
                    Cache.ForLocationAsync(searchItem.Location, Cancel)
                };

                //Give the test a timeout on deadlock.
                CancelSource.CancelAfter(TestCancellationTimeout);

                var results = await Task.WhenAll(tasks);

                //Ensure the test didn't timeout.
                Cancel.ThrowIfCancellationRequested();

                var result = Assert.Single(results.Distinct());
                Assert.Same(searchItem, result);
                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task ErrorAllowsSecondPopulationAsync()
            {
                Cache.SearchSpinTime = TimeSpan.FromMilliseconds(500);

                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var errorLoc = Create<ContentLocation>();
                var successItem = Cache.SearchData.First();

                Cache.SearchException = new ArgumentException();

                await Assert.ThrowsAsync<ArgumentException>(() => Cache.ForLocationAsync(errorLoc, Cancel));

                Cache.SearchException = null;

                var result = await Cache.ForLocationAsync(successItem.Location, Cancel);

                Assert.Same(successItem, result);
                Assert.Equal(2, Cache.SearchCalls);
            }
        }

        #endregion

        #region - ForIdAsync -

        public class ForIdAsync : ContentReferenceCacheBaseTest
        {
            [Fact]
            public async Task CachesByIdAsync()
            {
                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.SearchData.First();

                var result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Same(searchItem, result);

                //Test value cached.
                result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Same(searchItem, result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task CachesByLocationAsync()
            {
                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.SearchData.First();

                var result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Same(searchItem, result);

                //Test value cached.
                result = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Same(searchItem, result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task MultipleReturnValuesCachedAsync()
            {
                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>(),
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.SearchData.First();
                var extraItem = Cache.SearchData.Last();

                var result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Same(searchItem, result);

                //Test value cached.
                result = await Cache.ForIdAsync(extraItem.Id, Cancel);
                Assert.Same(extraItem, result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task NotFoundCachedAsync()
            {
                var notFoundItem = Create<ContentReferenceStub>();

                var result = await Cache.ForIdAsync(notFoundItem.Id, Cancel);
                Assert.Null(result);

                result = await Cache.ForIdAsync(notFoundItem.Id, Cancel);
                Assert.Null(result);

                Assert.Equal(1, Cache.SearchCalls);
            }

            // TODO: W-14187810 - Fix Flaky Test.
            // Increasing the timeout configuration helps when the test runs in a machine with limited resources.
            [Fact]
            public async Task ThreadSafePopulationAsync()
            {
                Cache.SearchSpinTime = TimeSpan.FromMilliseconds(500);

                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.SearchData.First();

                var tasks = new[]
                {
                    Cache.ForIdAsync(searchItem.Id, Cancel),
                    Cache.ForIdAsync(searchItem.Id, Cancel)
                };

                //Give the test a timeout on deadlock.
                CancelSource.CancelAfter(TestCancellationTimeout);

                var results = await Task.WhenAll(tasks);

                //Ensure the test didn't timeout.
                Cancel.ThrowIfCancellationRequested();

                var result = Assert.Single(results.Distinct());
                Assert.Same(searchItem, result);
                Assert.Equal(1, Cache.SearchCalls);
            }

            [Fact]
            public async Task ErrorAllowsSecondPopulationAsync()
            {
                Cache.SearchData = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var errorId = Create<Guid>();
                var successItem = Cache.SearchData.First();

                Cache.SearchException = new ArgumentException();

                await Assert.ThrowsAsync<ArgumentException>(() => Cache.ForIdAsync(errorId, Cancel));

                Cache.SearchException = null;

                var result = await Cache.ForIdAsync(successItem.Id, Cancel);

                Assert.Same(successItem, result);
                Assert.Equal(2, Cache.SearchCalls);
            }
        }

        #endregion
    }
}
