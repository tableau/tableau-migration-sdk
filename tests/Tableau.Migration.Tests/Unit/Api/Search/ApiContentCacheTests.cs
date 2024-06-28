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
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Tests.Unit.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Search
{
    public class ApiContentCacheTests
    {
        public abstract class ApiContentCacheTest : BulkCacheTest<ApiContentCache<TestContentType>, TestContentType>
        {
            protected readonly ConcurrentDictionary<ContentReferenceStub, TestContentType> InnerCache = new();

            protected readonly Mock<ApiContentCache<TestContentType>> MockCache;

            protected Mock<IContentReferenceCache> MockReferenceCache => MockCache.As<IContentReferenceCache>();

            public ApiContentCacheTest()
            {
                MockCache = Mock.Get(Cache);
            }

            protected override ApiContentCache<TestContentType> CreateCache()
                => new Mock<ApiContentCache<TestContentType>>(
                    MockSitesApiClient.Object, 
                    MockConfigReader.Object, 
                    InnerCache) 
                    { 
                        CallBase = true 
                    }
                    .Object;
        }

        public class ForLocationAsync : ApiContentCacheTest
        {
            [Fact]
            public async Task Returns_cached_content_when_found()
            {
                var reference = EndpointContent[0].ToStub();
                var content = new TestContentType(reference);

                MockReferenceCache.Setup(c => c.ForLocationAsync(reference.Location, Cancel)).ReturnsAsync(reference);

                InnerCache.GetOrAdd(reference, content);

                var result = await Cache.ForLocationAsync(content.Location, Cancel);

                Assert.Equal<IContentReference>(content, result);
            }

            [Fact]
            public async Task Returns_null_when_not_found()
            {
                MockReferenceCache.Setup(c => c.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((IContentReference?)null);

                var result = await Cache.ForLocationAsync(Create<ContentLocation>(), Cancel);

                Assert.Null(result);
            }
        }

        public class ForIdAsync : ApiContentCacheTest
        {
            [Fact]
            public async Task Returns_cached_content_when_found()
            {
                var reference = EndpointContent[0].ToStub();
                var content = new TestContentType(reference);

                MockReferenceCache.Setup(c => c.ForIdAsync(reference.Id, Cancel)).ReturnsAsync(reference);

                InnerCache.GetOrAdd(reference, content);

                var result = await Cache.ForIdAsync(content.Id, Cancel);

                Assert.Equal<IContentReference>(content, result);
            }

            [Fact]
            public async Task Returns_null_when_not_found()
            {
                MockReferenceCache.Setup(c => c.ForIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((IContentReference?)null);

                var result = await Cache.ForIdAsync(Create<Guid>(), Cancel);

                Assert.Null(result);
            }
        }

        public class ForReferenceAsync : ApiContentCacheTest
        {
            [Fact]
            public async Task Returns_cached_content_when_found()
            {
                var reference = EndpointContent[0].ToStub();
                var content = new TestContentType(reference);

                InnerCache.GetOrAdd(reference, content);

                var result = await Cache.ForReferenceAsync(reference, Cancel);

                Assert.Equal<IContentReference>(content, result);
            }

            [Fact]
            public async Task Returns_null_when_not_found()
            {
                InnerCache.Clear();

                var result = await Cache.ForReferenceAsync(Create<IContentReference>(), Cancel);

                Assert.Null(result);
            }
        }
    }
}
