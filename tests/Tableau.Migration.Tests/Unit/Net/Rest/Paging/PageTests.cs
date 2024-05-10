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

using Tableau.Migration.Net.Rest.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Paging
{
    public class PageTests
    {
        public abstract class PageTest : AutoFixtureTestBase
        { }

        public class Ctor : PageTest
        {
            [Fact]
            public void Sets_PageSize()
            {
                var size = Create<int>();

                var page = new Page(Create<int>(), size);

                Assert.Equal(size, page.PageSize);
            }

            [Fact]
            public void Sets_PageNumber()
            {
                var number = Create<int>();

                var page = new Page(number, Create<int>());

                Assert.Equal(number, page.PageNumber);
            }
        }
    }
}
