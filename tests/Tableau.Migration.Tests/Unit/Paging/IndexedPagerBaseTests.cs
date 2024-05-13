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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public class IndexedPagerBaseTests
    {
        public class TestPager : IndexedPagerBase<TestContentType>
        {
            public List<int> CalledPageNumbers { get; } = new();

            public TestPager(int pageSize)
                : base(pageSize)
            { }

            protected override Task<IPagedResult<TestContentType>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            {
                CalledPageNumbers.Add(pageNumber);

                var result = PagedResult<TestContentType>.Succeeded(ImmutableArray<TestContentType>.Empty, pageNumber, pageSize, 2 * pageSize, true);
                return Task.FromResult((IPagedResult<TestContentType>)result);
            }
        }

        public class NextPageAsync
        {
            private readonly CancellationToken _cancel = new();

            [Fact]
            public async Task StartsAtPageOneAsync()
            {
                var pager = new TestPager(100);
                await pager.NextPageAsync(_cancel);

                Assert.Equal(new[] { 1 }, pager.CalledPageNumbers);
            }

            [Fact]
            public async Task IncrementsPageNumberAsync()
            {
                var pager = new TestPager(100);

                await pager.NextPageAsync(_cancel);
                await pager.NextPageAsync(_cancel);

                Assert.Equal(new[] { 1, 2 }, pager.CalledPageNumbers);
            }
        }
    }
}
