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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Search
{
    public class CachedContentReferenceFinderTests
    {
        public class CachedContentReferenceFinderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IContentReferenceCache> MockCache;
            protected readonly CachedContentReferenceFinder<TestContentType> Finder;

            public CachedContentReferenceFinderTest()
            {
                MockCache = Freeze<Mock<IContentReferenceCache>>();
                Finder = Create<CachedContentReferenceFinder<TestContentType>>();
            }
        }

        public class FindByIdAsync : CachedContentReferenceFinderTest
        {
            [Fact]
            public async Task CallsCacheAsync()
            {
                var id = Guid.NewGuid();

                var cacheResult = Create<IContentReference>();
                MockCache.Setup(x => x.ForIdAsync(id, Cancel))
                    .ReturnsAsync(cacheResult);

                var result = await Finder.FindByIdAsync(id, Cancel);

                Assert.Same(cacheResult, result);
                MockCache.Verify(x => x.ForIdAsync(id, Cancel), Times.Once);
            }
        }
    }
}
