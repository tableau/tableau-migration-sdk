//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Engine.Caching;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Caching
{
    public sealed class ConfigurableMigrationCacheBaseTests
    {
        #region - Test Classes -

        public class TestCache : ConfigurableMigrationCacheBase<Guid, TestContentType>
        {
            internal const string CACHE_CONFIG_KEY = "test";

            protected override MemoryCacheOptions DefaultMemoryCacheOptions => PublicDefaultMemoryCacheOptions;

            public MemoryCacheOptions PublicDefaultMemoryCacheOptions { get; set; } = new()
            {
                TrackStatistics = true
            };

            public MemoryCache PublicMemoryCache => MemoryCache;

            public Dictionary<Guid, IResult<TestContentType>> CacheMissData { get; set; } = new();

            public int MissCount { get; private set; }

            public TestCache(IConfigReader config)
                : base(config, CACHE_CONFIG_KEY)
            { }

            protected override Task<IResult<TestContentType>> FindCacheMissAsync(Guid key, CancellationToken cancel)
            {
                MissCount++;
                return Task.FromResult(CacheMissData[key]);
            }

            public MemoryCacheOptions PublicMergeOptions(CacheOptions options, MemoryCacheOptions defaultOptions)
                => MergeOptions(options, defaultOptions);

            public MemoryCacheOptions PublicBuildCacheOptions(IConfigReader configReader, string cacheKey)
                => BuildCacheOptions(configReader, cacheKey);

            public void PublicAdd(Guid key, IResult<TestContentType> findResult)
                => Add(key, findResult);
        }

        public abstract class ConfigurableMigrationCacheBaseTest : AutoFixtureTestBase
        {
            protected readonly Mock<IConfigReader> MockConfigReader;

            protected MigrationSdkOptions Config { get; set; }

            protected readonly TestCache Cache;

            protected virtual void SetConfig() { }

            public ConfigurableMigrationCacheBaseTest()
            {
                Config = new();

                MockConfigReader = Freeze<Mock<IConfigReader>>();
                MockConfigReader.Setup(x => x.Get()).Returns(() => Config);

                SetConfig();

                Cache = Create<TestCache>();
            }
        }

        #endregion

        #region - MergeOptions -

        public sealed class MergeOptions : ConfigurableMigrationCacheBaseTest
        {
            [Fact]
            public void UsesDefaultOptions()
            {
                var configOptions = new CacheOptions()
                {
                    SizeLimit = null
                };

                var defaultOptions = new MemoryCacheOptions()
                {
                    SizeLimit = 50,
                    TrackStatistics = true
                };

                var result = Cache.PublicMergeOptions(configOptions, defaultOptions);

                Assert.Equal(defaultOptions.SizeLimit, result.SizeLimit);
                Assert.True(result.TrackStatistics);
            }

            [Fact]
            public void AllowsConfigOverride()
            {
                var configOptions = new CacheOptions()
                {
                    SizeLimit = 500
                };

                var defaultOptions = new MemoryCacheOptions()
                {
                    SizeLimit = 50,
                    TrackStatistics = true
                };

                var result = Cache.PublicMergeOptions(configOptions, defaultOptions);

                Assert.Equal(configOptions.SizeLimit, result.SizeLimit);
                Assert.True(result.TrackStatistics);
            }
        }

        #endregion

        #region - BuildCacheOptions -

        public sealed class BuildCacheOptions : ConfigurableMigrationCacheBaseTest
        {
            [Fact]
            public void LoadsConfig()
            {
                Config = new()
                {
                    Caches =
                    {
                        { TestCache.CACHE_CONFIG_KEY, new() { SizeLimit = 47 } }
                    }
                };

                var mockReader = new Mock<IConfigReader>();
                mockReader.Setup(x => x.Get()).Returns(() => Config);

                var result = Cache.PublicBuildCacheOptions(mockReader.Object, TestCache.CACHE_CONFIG_KEY);

                Assert.Equal(Config.Caches[TestCache.CACHE_CONFIG_KEY].SizeLimit, result.SizeLimit);
                mockReader.Verify(x => x.Get(), Times.Once);
            }

            [Fact]
            public void NoConfig()
            {
                Cache.PublicDefaultMemoryCacheOptions = new()
                {
                    SizeLimit = 54
                };

                var mockReader = new Mock<IConfigReader>();
                mockReader.Setup(x => x.Get()).Returns(() => Config);

                var result = Cache.PublicBuildCacheOptions(mockReader.Object, TestCache.CACHE_CONFIG_KEY);

                Assert.Equal(Cache.PublicDefaultMemoryCacheOptions.SizeLimit, result.SizeLimit);
                mockReader.Verify(x => x.Get(), Times.Once);
            }
        }

        #endregion

        #region - Add -

        public sealed class Add : ConfigurableMigrationCacheBaseTest
        {
            protected override void SetConfig()
            {
                Config.Caches[TestCache.CACHE_CONFIG_KEY] = new()
                {
                    SizeLimit = 10
                };
            }

            [Fact]
            public void AddsCacheValue()
            {
                var id = Guid.NewGuid();
                var value = Result<TestContentType>.Succeeded(new TestContentType());

                Cache.PublicAdd(id, value);

                Assert.True(Cache.PublicMemoryCache.TryGetValue<IResult<TestContentType>>(id, out var result));
                Assert.NotNull(result);
                Assert.Same(value, result);

                var stats = Cache.PublicMemoryCache.GetCurrentStatistics();
                Assert.NotNull(stats);
                Assert.Equal(1, stats.CurrentEntryCount);
                Assert.Equal(1, stats.CurrentEstimatedSize);
            }
        }

        #endregion

        #region - Add (Public) -

        public sealed class AddPublic : ConfigurableMigrationCacheBaseTest
        {
            [Fact]
            public void AddsCacheValue()
            {
                var id = Guid.NewGuid();
                var value = new TestContentType();

                Cache.Add(id, value);

                Assert.True(Cache.PublicMemoryCache.TryGetValue<IResult<TestContentType>>(id, out var result));
                Assert.NotNull(result);

                result.AssertSuccess();
                Assert.Same(value, result.Value);
            }
        }

        #endregion

        #region - GetOrAddAsync -

        public sealed class GetOrAddAsync : ConfigurableMigrationCacheBaseTest
        {
            [Fact]
            public async Task CachesValueAsync()
            {
                var id = Guid.NewGuid();
                var value = Result<TestContentType>.Succeeded(new TestContentType());

                Cache.CacheMissData = new() { { id, value } };

                var result = await Cache.GetOrAddAsync(id, Cancel);

                Assert.Same(value, result);
                Assert.Equal(1, Cache.MissCount);

                result = await Cache.GetOrAddAsync(id, Cancel);

                Assert.Same(value, result);
                Assert.Equal(1, Cache.MissCount);
            }

            [Fact]
            public async Task NullCacheValuesCountAsMissAsync()
            {
                var id = Guid.NewGuid();
                var value = Result<TestContentType>.Succeeded(new TestContentType());

                Cache.CacheMissData = new() { { id, value } };

                Cache.PublicMemoryCache.Set<IResult<TestContentType>?>(id, null);

                var result = await Cache.GetOrAddAsync(id, Cancel);

                Assert.Same(value, result);
                Assert.Equal(1, Cache.MissCount);
            }
        }

        #endregion
    }
}
