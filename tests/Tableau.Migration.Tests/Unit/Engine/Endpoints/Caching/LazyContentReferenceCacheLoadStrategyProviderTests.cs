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

using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Endpoints.Caching;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Caching
{
    public sealed class LazyContentReferenceCacheLoadStrategyProviderTests
    {
        public sealed class GetDestinationCacheLoadStrategy : AutoFixtureTestBase
        {
            [Fact]
            public void ReturnsLazyStrategy()
            {
                var s = new LazyContentReferenceCacheLoadStrategyProvider<TestContentType>().GetDestinationCacheLoadStrategy();
                Assert.Same(LazyContentReferenceCacheLoadStrategyProvider<TestContentType>.STRATEGY, s);
                Assert.IsType<LazyContentReferenceCacheLoadStrategy<TestContentType>>(s);
            }
        }

        public sealed class GetSourceCacheLoadStrategy : AutoFixtureTestBase
        {
            [Fact]
            public void ReturnsLazyStrategy()
            {
                var s = new LazyContentReferenceCacheLoadStrategyProvider<TestContentType>().GetSourceCacheLoadStrategy();
                Assert.Same(LazyContentReferenceCacheLoadStrategyProvider<TestContentType>.STRATEGY, s);
                Assert.IsType<LazyContentReferenceCacheLoadStrategy<TestContentType>>(s);
            }
        }
    }
}
