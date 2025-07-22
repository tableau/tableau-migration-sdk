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

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public sealed class CallbackPagerTests
    {
        public sealed class GetPageAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task CallsCallbackAsync()
            {
                int calls = 0;
                var allItems = CreateMany<int>(300).ToImmutableArray();

                Task<IPagedResult<int>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
                {
                    calls++;
                    var page = PagedResult<int>.Succeeded(allItems.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToImmutableArray(), pageNumber, pageSize, allItems.Length, pageNumber == 3);
                    return Task.FromResult<IPagedResult<int>>(page);
                }

                IPager<int> pager = new CallbackPager<int>(GetPageAsync, 100);

                var result = await pager.GetAllPagesAsync(Cancel);

                Assert.Equal(3, calls);
            }
        }
    }
}
