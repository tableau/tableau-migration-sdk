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
using System.Collections.Immutable;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public class PagedResultTests
    {
        public class Succeeded : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var items = CreateMany<object>().ToImmutableList();

                var r = PagedResult<object>.Succeeded(items, 1, 2, 3, true);

                r.AssertSuccess();

                Assert.Equal(items, r.Value);
                Assert.Equal(1, r.PageNumber);
                Assert.Equal(2, r.PageSize);
                Assert.Equal(3, r.TotalCount);
                Assert.True(r.FetchedAllPages);
            }
        }

        public class Failed
        {
            public class SingleException : AutoFixtureTestBase
            {
                [Fact]
                public void Initializes()
                {
                    var error = new Exception();

                    var r = PagedResult<object>.Failed(error);

                    r.AssertFailure();

                    Assert.Same(error, Assert.Single(r.Errors));
                    Assert.Equal(0, r.PageNumber);
                    Assert.Equal(0, r.PageSize);
                    Assert.Equal(0, r.TotalCount);
                    Assert.True(r.FetchedAllPages);
                }
            }

            public class ExceptionCollection : AutoFixtureTestBase
            {
                [Fact]
                public void Initializes()
                {
                    var errors = CreateMany<Exception>().ToImmutableList();

                    var r = PagedResult<object>.Failed(errors);

                    r.AssertFailure();

                    Assert.Equal(errors, r.Errors);
                    Assert.Equal(0, r.PageNumber);
                    Assert.Equal(0, r.PageSize);
                    Assert.Equal(0, r.TotalCount);
                    Assert.True(r.FetchedAllPages);
                }
            }
        }
    }
}
