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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Search
{
    public sealed class ContentReferenceCacheBaseTests
    {
        #region - Test Classes -

        public class TestContentReferenceStore<TContent> : IContentReferenceStore<TContent>
            where TContent : IContentReference
        {
            public IEnumerable<TContent> Data { get; set; }
                = Enumerable.Empty<TContent>();

            public TimeSpan? LoadSpinTime { get; set; }

            public Exception? LoadException { get; set; }

            public int LoadCalls { get; private set; }

            public async ValueTask<IImmutableList<TContent>> LoadAllAsync(CancellationToken cancel)
            {
                LoadCalls++;
                if (LoadSpinTime is not null)
                {
                    await Task.Delay(LoadSpinTime.Value, cancel);
                }

                if (LoadException is not null)
                {
                    throw LoadException;
                }

                return Data.ToImmutableArray();
            }

            public async ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(ContentLocation searchLocation, CancellationToken cancel)
            {
                LoadCalls++;
                if (LoadSpinTime is not null)
                {
                    await Task.Delay(LoadSpinTime.Value, cancel);
                }

                if (LoadException is not null)
                {
                    throw LoadException;
                }

                return new(Data.Where(i => i.Location == searchLocation).ToImmutableArray());
            }

            public async ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(Guid searchId, CancellationToken cancel)
            {
                LoadCalls++;
                if (LoadSpinTime is not null)
                {
                    await Task.Delay(LoadSpinTime.Value, cancel);
                }

                if (LoadException is not null)
                {
                    throw LoadException;
                }

                return new(Data.Where(i => i.Id == searchId).ToImmutableArray());
            }

            public async ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(string contentUrl, CancellationToken cancel)
            {
                LoadCalls++;
                if (LoadSpinTime is not null)
                {
                    await Task.Delay(LoadSpinTime.Value, cancel);
                }

                if (LoadException is not null)
                {
                    throw LoadException;
                }

                return new(Data.Where(i => i.ContentUrl == contentUrl).ToImmutableArray());
            }
        }

        public class TestContentReferenceCache<TContent> : ContentReferenceCacheBase<TContent>
            where TContent : IContentReference
        {
            public TestContentReferenceStore<TContent> TestStore => (TestContentReferenceStore<TContent>)base.Store;

            protected override string Name => "Test";

            public TestContentReferenceCache(ILogger<TestContentReferenceCache<TContent>> logger)
                : base(new BulkContentReferenceCacheLoadStrategy<TContent>(), new TestContentReferenceStore<TContent>(), logger)
            { }
        }

        public class ContentReferenceCacheBaseTest : AutoFixtureTestBase
        {
            protected readonly TestContentReferenceCache<ContentReferenceStub> Cache;

            public ContentReferenceCacheBaseTest()
            {
                Cache = Create<TestContentReferenceCache<ContentReferenceStub>>();
            }
        }

        #endregion

        #region - ForLocationAsync -

        public sealed class ForLocationAsync : ContentReferenceCacheBaseTest
        {
            [Fact]
            public async Task CachesByLocationAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

                var result = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                var result2 = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Same(result, result2);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task CachesByIdAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

                var result = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                var result2 = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Same(result, result2);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task MultipleReturnValuesCachedAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>(),
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();
                var extraItem = Cache.TestStore.Data.Last();

                var result = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                result = await Cache.ForLocationAsync(extraItem.Location, Cancel);
                Assert.Equal(extraItem, result);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task NotFoundCachedAsync()
            {
                var notFoundItem = Create<ContentReferenceStub>();

                var result = await Cache.ForLocationAsync(notFoundItem.Location, Cancel);
                Assert.Null(result);

                result = await Cache.ForLocationAsync(notFoundItem.Location, Cancel);
                Assert.Null(result);

                Assert.Equal(2, Cache.TestStore.LoadCalls);
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

                Assert.Equal(2, Cache.TestStore.LoadCalls);

                result = await Cache.ForLocationAsync(notFoundItem2.Location, Cancel);
                Assert.Null(result);

                Assert.Equal(3, Cache.TestStore.LoadCalls);
            }

            // TODO: W-14187810 - Fix Flaky Test.
            // Increasing the timeout configuration helps when the test runs in a machine with limited resources.
            [Fact]
            public async Task ThreadSafePopulationAsync()
            {
                Cache.TestStore.LoadSpinTime = TimeSpan.FromMilliseconds(500);

                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

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
                Assert.Equal(searchItem, result);
                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task ErrorAllowsSecondPopulationAsync()
            {
                Cache.TestStore.LoadSpinTime = TimeSpan.FromMilliseconds(500);

                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var errorLoc = Create<ContentLocation>();
                var successItem = Cache.TestStore.Data.First();

                Cache.TestStore.LoadException = new ArgumentException();

                await Assert.ThrowsAsync<ArgumentException>(() => Cache.ForLocationAsync(errorLoc, Cancel));

                Cache.TestStore.LoadException = null;

                var result = await Cache.ForLocationAsync(successItem.Location, Cancel);

                Assert.Equal(successItem, result);
                Assert.Equal(2, Cache.TestStore.LoadCalls);
            }
        }

        #endregion

        #region - ForIdAsync -

        public sealed class ForIdAsync : ContentReferenceCacheBaseTest
        {
            [Fact]
            public async Task CachesByIdAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

                var result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                var result2 = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Equal(result, result2);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task DoesNotCacheByEmptyIdAsync()
            {
                Cache.TestStore.Data = new[]
                {
                   new ContentReferenceStub(Guid.Empty,string.Empty, Create<ContentLocation>()),
                   new ContentReferenceStub(Guid.Empty,string.Empty, Create<ContentLocation>())
                };

                var searchItem = Cache.TestStore.Data.First();

                var result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Null(result);

                //Test value not cached.
                result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Null(result);

                var searchItem2 = Cache.TestStore.Data.Last();

                var result2 = await Cache.ForIdAsync(searchItem2.Id, Cancel);
                Assert.Null(result);

                //Test value not cached.
                result = await Cache.ForIdAsync(searchItem2.Id, Cancel);
                Assert.Null(result);

                Assert.Equal(0, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task CachesByLocationAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

                var result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                var result2 = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Same(result, result2);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task MultipleReturnValuesCachedAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>(),
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();
                var extraItem = Cache.TestStore.Data.Last();

                var result = await Cache.ForIdAsync(searchItem.Id, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                var result2 = await Cache.ForIdAsync(extraItem.Id, Cancel);
                Assert.Equal(extraItem, result2);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task NotFoundCachedAsync()
            {
                var notFoundItem = Create<ContentReferenceStub>();

                var result = await Cache.ForIdAsync(notFoundItem.Id, Cancel);
                Assert.Null(result);

                result = await Cache.ForIdAsync(notFoundItem.Id, Cancel);
                Assert.Null(result);

                Assert.Equal(2, Cache.TestStore.LoadCalls);
            }

            // TODO: W-14187810 - Fix Flaky Test.
            // Increasing the timeout configuration helps when the test runs in a machine with limited resources.
            [Fact]
            public async Task ThreadSafePopulationAsync()
            {
                Cache.TestStore.LoadSpinTime = TimeSpan.FromMilliseconds(500);

                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

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
                Assert.Equal(searchItem, result);
                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task ErrorAllowsSecondPopulationAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var errorId = Create<Guid>();
                var successItem = Cache.TestStore.Data.First();

                Cache.TestStore.LoadException = new ArgumentException();

                await Assert.ThrowsAsync<ArgumentException>(() => Cache.ForIdAsync(errorId, Cancel));

                Cache.TestStore.LoadException = null;

                var result = await Cache.ForIdAsync(successItem.Id, Cancel);

                Assert.Equal(successItem, result);
                Assert.Equal(2, Cache.TestStore.LoadCalls);
            }
        }

        #endregion

        #region - ForContentUrlAsync -

        public sealed class ForContentUrlAsync : ContentReferenceCacheBaseTest
        {
            [Fact]
            public async Task CachesByContentUrlAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

                var result = await Cache.ForContentUrlAsync(searchItem.ContentUrl, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                var result2 = await Cache.ForContentUrlAsync(searchItem.ContentUrl, Cancel);
                Assert.Equal(result, result2);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task DoesNotCacheByEmptyStringAsync()
            {
                Cache.TestStore.Data = new[]
                {
                   new ContentReferenceStub(Guid.Empty,string.Empty, Create<ContentLocation>()),
                   new ContentReferenceStub(Guid.Empty,string.Empty, Create<ContentLocation>())
                };

                var searchItem = Cache.TestStore.Data.First();

                var result = await Cache.ForContentUrlAsync(searchItem.ContentUrl, Cancel);
                Assert.Null(result);

                //Test value not cached.
                result = await Cache.ForContentUrlAsync(searchItem.ContentUrl, Cancel);
                Assert.Null(result);

                var searchItem2 = Cache.TestStore.Data.Last();

                var result2 = await Cache.ForContentUrlAsync(searchItem2.ContentUrl, Cancel);
                Assert.Null(result);

                //Test value not cached.
                result = await Cache.ForContentUrlAsync(searchItem2.ContentUrl, Cancel);
                Assert.Null(result);

                Assert.Equal(0, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task CachesByLocationAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

                var result = await Cache.ForContentUrlAsync(searchItem.ContentUrl, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                var result2 = await Cache.ForLocationAsync(searchItem.Location, Cancel);
                Assert.Same(result, result2);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task MultipleReturnValuesCachedAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>(),
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();
                var extraItem = Cache.TestStore.Data.Last();

                var result = await Cache.ForContentUrlAsync(searchItem.ContentUrl, Cancel);
                Assert.Equal(searchItem, result);

                //Test value cached.
                var result2 = await Cache.ForContentUrlAsync(extraItem.ContentUrl, Cancel);
                Assert.Equal(extraItem, result2);

                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task NotFoundCachedAsync()
            {
                var notFoundItem = Create<ContentReferenceStub>();

                var result = await Cache.ForContentUrlAsync(notFoundItem.ContentUrl, Cancel);
                Assert.Null(result);

                result = await Cache.ForContentUrlAsync(notFoundItem.ContentUrl, Cancel);
                Assert.Null(result);

                Assert.Equal(2, Cache.TestStore.LoadCalls);
            }

            // TODO: W-14187810 - Fix Flaky Test.
            // Increasing the timeout configuration helps when the test runs in a machine with limited resources.
            [Fact]
            public async Task ThreadSafePopulationAsync()
            {
                Cache.TestStore.LoadSpinTime = TimeSpan.FromMilliseconds(500);

                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var searchItem = Cache.TestStore.Data.First();

                var tasks = new[]
                {
                    Cache.ForContentUrlAsync(searchItem.ContentUrl, Cancel),
                    Cache.ForContentUrlAsync(searchItem.ContentUrl, Cancel)
                };

                //Give the test a timeout on deadlock.
                CancelSource.CancelAfter(TestCancellationTimeout);

                var results = await Task.WhenAll(tasks);

                //Ensure the test didn't timeout.
                Cancel.ThrowIfCancellationRequested();

                var result = Assert.Single(results.Distinct());
                Assert.Equal(searchItem, result);
                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }

            [Fact]
            public async Task ErrorAllowsSecondPopulationAsync()
            {
                Cache.TestStore.Data = new[]
                {
                    Create<ContentReferenceStub>()
                };

                var errorId = Create<Guid>();
                var successItem = Cache.TestStore.Data.First();

                Cache.TestStore.LoadException = new ArgumentException();

                await Assert.ThrowsAsync<ArgumentException>(() => Cache.ForIdAsync(errorId, Cancel));

                Cache.TestStore.LoadException = null;

                var result = await Cache.ForContentUrlAsync(successItem.ContentUrl, Cancel);

                Assert.Equal(successItem, result);
                Assert.Equal(2, Cache.TestStore.LoadCalls);
            }
        }

        #endregion

        #region - GetAllAsync -

        public sealed class GetAllAsync : ContentReferenceCacheBaseTest
        {
            [Fact]
            public async Task SearchesAllAndCachesAsync()
            {
                Cache.TestStore.Data = CreateMany<ContentReferenceStub>();
                var result1 = await Cache.GetAllAsync(Cancel);

                Assert.Equal(Cache.TestStore.Data, result1);
                Assert.Equal(1, Cache.TestStore.LoadCalls);
                
                var result2 = await Cache.GetAllAsync(Cancel);
                Assert.Equal(result1, result2);
                Assert.Equal(1, Cache.TestStore.LoadCalls);
            }
        }

        #endregion
    }
}
