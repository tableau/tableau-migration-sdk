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
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Schedules
{
    public class VolatileCacheTests
    {
        public abstract class VolatileCacheTest<TKey, TContent> : AutoFixtureTestBase
            where TKey : struct
            where TContent : class
        {
            internal VolatileCache<TKey, TContent>? Cache { get; set; }
        }

        public class SingleThread : VolatileCacheTest<(ExtractRefreshContentType, Guid), ImmutableList<ICloudExtractRefreshTask>>
        {
            [Fact]
            public async Task EmptyList_LoadsOnce()
            {
                var loaded = 0;
                Cache = new(
                    cancel =>
                    {
                        loaded++;
                        return Task.FromResult(
                            new Dictionary<(ExtractRefreshContentType, Guid), ImmutableList<ICloudExtractRefreshTask>>());
                    });

                var result1 = await Cache.GetAndRelease((ExtractRefreshContentType.DataSource, Guid.NewGuid()), Cancel);

                var result2 = await Cache.GetAndRelease((ExtractRefreshContentType.Workbook, Guid.NewGuid()), Cancel);

                Assert.Null(result1);
                Assert.Null(result2);
                Assert.Equal(1, loaded);
            }

            [Fact]
            public async Task SingleItem_LoadsAndReturnsOnce()
            {
                var loaded = 0;
                var id = Guid.NewGuid();
                var list = CreateMany<ICloudExtractRefreshTask>().ToImmutableList();
                Cache = new(
                    cancel =>
                    {
                        loaded++;
                        return Task.FromResult(
                            new Dictionary<(ExtractRefreshContentType, Guid), ImmutableList<ICloudExtractRefreshTask>>
                            {
                                [(ExtractRefreshContentType.DataSource, id)] = list
                            });
                    });


                var result1 = await Cache.GetAndRelease((ExtractRefreshContentType.DataSource, id), Cancel);

                var result2 = await Cache.GetAndRelease((ExtractRefreshContentType.DataSource, id), Cancel);

                Assert.NotNull(result1);
                Assert.Equal(list, result1);
                Assert.Null(result2);
                Assert.Equal(1, loaded);
            }

            [Fact]
            public async Task MultipleItems_LoadsAndReturnsOnce()
            {
                var loaded = 0;
                var id1 = Guid.NewGuid();
                var list1 = CreateMany<ICloudExtractRefreshTask>().ToImmutableList();
                var id2 = Guid.NewGuid();
                var list2 = CreateMany<ICloudExtractRefreshTask>().ToImmutableList();
                var id3 = Guid.NewGuid();
                var list3 = CreateMany<ICloudExtractRefreshTask>().ToImmutableList();
                Cache = new(
                    cancel =>
                    {
                        loaded++;
                        return Task.FromResult(
                            new Dictionary<(ExtractRefreshContentType, Guid), ImmutableList<ICloudExtractRefreshTask>>
                            {
                                [(ExtractRefreshContentType.DataSource, id1)] = list1,
                                [(ExtractRefreshContentType.Workbook, id2)] = list2,
                                [(ExtractRefreshContentType.DataSource, id3)] = list3,
                            });
                    });

                var result1 = await Cache.GetAndRelease((ExtractRefreshContentType.DataSource, id1), Cancel);

                var result2 = await Cache.GetAndRelease((ExtractRefreshContentType.Workbook, id2), Cancel);

                var emptyResult = await Cache.GetAndRelease((ExtractRefreshContentType.DataSource, Guid.NewGuid()), Cancel);

                Assert.NotNull(result1);
                Assert.Equal(list1, result1);
                Assert.NotNull(result2);
                Assert.Equal(list2, result2);
                Assert.Null(emptyResult);
                Assert.Equal(1, loaded);
            }
        }

        public class MultiThread : VolatileCacheTest<Guid, ICloudExtractRefreshTask>
        {
            [Fact]
            public async Task EmptyList_LoadsOnce()
            {
                var loaded = 0;
                Cache = new(
                    cancel =>
                    {
                        loaded++;
                        return Task.FromResult(
                            new Dictionary<Guid, ICloudExtractRefreshTask>());
                    });

                var totalThreads = 127;
                var tasks = Enumerable
                    .Range(1, totalThreads)
                    .Select(x => Cache
                        .GetAndRelease(
                            Guid.NewGuid(),
                            Cancel))
                    .ToList();
                var results = await Task.WhenAll(tasks);

                var notNullList = results.Where(result => result is not null).ToList();
                var nullList = results.Where(result => result is null).ToList();

                Assert.Empty(notNullList);
                Assert.Equal(totalThreads, nullList.Count);
                Assert.Equal(1, loaded);
            }

            [Fact]
            public async Task SingleItem_LoadsAndReturnsOnce()
            {
                var loaded = 0;
                var item = Create<ICloudExtractRefreshTask>();
                var id = item.Id;
                Cache = new(
                    cancel =>
                    {
                        loaded++;
                        return Task.FromResult(
                            new Dictionary<Guid, ICloudExtractRefreshTask>
                            {
                                [id] = item
                            });
                    });
                var totalThreads = 143;
                var tasks = Enumerable
                    .Range(1, totalThreads)
                    .Select(x => Cache
                        .GetAndRelease(
                            id,
                            Cancel))
                    .ToList();
                var results = await Task.WhenAll(tasks);

                var notNullList = results.Where(result => result is not null).ToList();
                var nullList = results.Where(result => result is null).ToList();

                Assert.Single(notNullList);
                Assert.Equal(totalThreads - 1, nullList.Count);
                Assert.Same(item, notNullList.First());
                Assert.Equal(1, loaded);
            }

            [Fact]
            public async Task MultipleItems_LoadsAndReturnsOnce()
            {
                var loaded = 0;
                var item1 = Create<ICloudExtractRefreshTask>();
                var id1 = item1.Id;

                var item2 = Create<ICloudExtractRefreshTask>();
                var id2 = item2.Id;

                var item3 = Create<ICloudExtractRefreshTask>();
                var id3 = item3.Id;
                Cache = new(
                    cancel =>
                    {
                        loaded++;
                        return Task.FromResult(
                            new Dictionary<Guid, ICloudExtractRefreshTask>
                            {
                                [id1] = item1,
                                [id2] = item2,
                                [id3] = item3
                            });
                    });
                var totalThreads = 159;
                var tasks = Enumerable
                    .Range(1, totalThreads)
                    .Select(x => Cache
                        .GetAndRelease(
                            x % 3 != 0
                                ? x % 3 != 1
                                    ? Guid.NewGuid()
                                    : id2
                                : id1,
                            Cancel))
                    .ToList();

                var results = await Task.WhenAll(tasks);
                var id1Result = results.First(result => result is not null && result.Id == id1);
                var id2Result = results.First(result => result is not null && result.Id == id2);
                var emptyResults = results.Where(result => result is null).ToList();

                Assert.NotNull(id1Result);
                Assert.Equal(item1, id1Result);
                Assert.NotNull(id2Result);
                Assert.Equal(item2, id2Result);
                Assert.Equal(totalThreads - 2, emptyResults.Count);
                Assert.Equal(1, loaded);
            }
        }
    }
}
