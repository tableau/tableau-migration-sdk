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

using Moq;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Paging
{
    public class PageBuilderTests
    {
        public abstract class PageBuilderTest : AutoFixtureTestBase
        {
            internal readonly PageBuilder Builder = new();
        }

        public class IsEmpty : PageBuilderTest
        {
            [Fact]
            public void True()
            {
                Assert.True(Builder.IsEmpty);
            }

            [Fact]
            public void False()
            {
                var page = Create<Page>();

                Builder.SetPage(page);

                Assert.False(Builder.IsEmpty);
            }
        }

        public class Build : PageBuilderTest
        {
            [Fact]
            public void Builds_page()
            {
                var page = Create<Page>();

                Builder.SetPage(page);

                Assert.Equal($"pageSize={page.PageSize}&pageNumber={page.PageNumber}", Builder.Build());
            }
        }

        public class AppendQueryString : PageBuilderTest
        {
            protected readonly Mock<IQueryStringBuilder> MockQuery = new();

            [Fact]
            public void Skips_when_empty()
            {
                Assert.True(Builder.IsEmpty);

                Builder.AppendQueryString(MockQuery.Object);

                MockQuery.VerifyNoOtherCalls();
            }

            [Fact]
            public void Appends()
            {
                var page = Create<Page>();

                Builder.SetPage(page);

                Assert.False(Builder.IsEmpty);

                Builder.AppendQueryString(MockQuery.Object);

                MockQuery.Verify(q => q.AddOrUpdate("pageSize", page.PageSize.ToString()), Times.Once);
                MockQuery.Verify(q => q.AddOrUpdate("pageNumber", page.PageNumber.ToString()), Times.Once);
            }
        }
    }
}
