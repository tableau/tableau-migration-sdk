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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Engine.Endpoints.Caching;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Caching
{
    public sealed class TableauApiEndpointViewCacheTests
    {
        public abstract class TableauApiEndpointViewCacheTest : AutoFixtureTestBase
        {
            protected readonly Mock<IViewsApiClient> MockViewsApiClient;

            protected readonly TableauApiEndpointViewCache Cache;

            public TableauApiEndpointViewCacheTest()
            {
                MockViewsApiClient = Freeze<Mock<IViewsApiClient>>();

                Cache = Create<TableauApiEndpointViewCache>();
            }
        }

        public sealed class FindCacheMissAsync : TableauApiEndpointViewCacheTest
        {
            [Fact]
            public async Task FindsFromApiAsync()
            {
                var id = Guid.NewGuid();

                var result = await Cache.GetOrAddAsync(id, Cancel);
                result = await Cache.GetOrAddAsync(id, Cancel);

                MockViewsApiClient.Verify(x => x.GetByIdAsync(id, Cancel), Times.Once);
            }
        }
    }
}
