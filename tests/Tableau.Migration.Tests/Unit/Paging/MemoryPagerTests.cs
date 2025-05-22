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
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public sealed class MemoryPagerTests
    {
        public sealed class NextPageAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task PagesThroughCollectionAsync()
            {
                int count = 105;
                int pageSize = 10;

                var collection = CreateMany<IUser>(count).ToImmutableArray();

                var pager = new MemoryPager<IUser>(collection, pageSize);

                for (int i = 0; i < count / pageSize + 1; i++)
                {
                    var pageResult = await pager.NextPageAsync(Cancel);

                    pageResult.AssertSuccess();
                    Assert.Equal(pageSize, pageResult.PageSize);
                    Assert.Equal(i + 1, pageResult.PageNumber);
                    Assert.Equal(count, pageResult.TotalCount);
                    Assert.Equal(collection.Skip(i * pageSize).Take(pageSize), pageResult.Value);
                    if (i == count / pageSize)
                        Assert.True(pageResult.FetchedAllPages);
                    else
                        Assert.False(pageResult.FetchedAllPages);
                }
            }

            [Fact]
            public async Task GetFailsAsync()
            {
                var failureResult = Result<IReadOnlyCollection<IUser>>.Failed(CreateMany<Exception>());

                var pager = new MemoryPager<IUser>((c) => Task.FromResult<IResult<IReadOnlyCollection<IUser>>>(failureResult), 10);

                var pageResult = await pager.NextPageAsync(Cancel);

                pageResult.AssertFailure();
                Assert.Equal(failureResult.Errors, pageResult.Errors);
            }
        }
    }
}
