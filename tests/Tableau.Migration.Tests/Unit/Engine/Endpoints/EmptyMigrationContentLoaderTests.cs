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

using System.Threading.Tasks;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public sealed class EmptyMigrationContentLoaderTests
    {
        public sealed class GetPager : AutoFixtureTestBase
        {
            [Fact]
            public async Task BuildsEmptyPagerAsync()
            {
                var loader = new EmptyMigrationContentLoader<TestContentType>();
                var pager = loader.GetMigrationContentPager(55);

                var memoryPager = Assert.IsType<MemoryPager<TestContentType>>(pager);

                var page = await memoryPager.NextPageAsync(Cancel);

                page.AssertSuccess();
                Assert.True(page.FetchedAllPages);
                Assert.NotNull(page.Value);
                Assert.Empty(page.Value);
                Assert.Equal(55, page.PageSize);
                Assert.Equal(1, page.PageNumber);
            }
        }
    }
}
