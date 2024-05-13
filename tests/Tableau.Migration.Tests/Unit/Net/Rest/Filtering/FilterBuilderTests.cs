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

using System.Linq;
using Moq;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest.Filtering;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Filtering
{
    public class FilterBuilderTests
    {
        public abstract class FilterBuilderTest : AutoFixtureTestBase
        {
            internal readonly FilterBuilder Builder = new();
        }

        public class IsEmpty : FilterBuilderTest
        {
            [Fact]
            public void True()
            {
                Assert.True(Builder.IsEmpty);
            }

            [Fact]
            public void False()
            {
                var filter = Create<Filter>();

                Builder.AddFilter(filter);

                Assert.False(Builder.IsEmpty);
            }
        }

        public class Build : FilterBuilderTest
        {
            [Fact]
            public void Builds_single_filter()
            {
                var filter = Create<Filter>();

                Builder.AddFilter(filter);

                Assert.Equal($"filter={filter.Expression}", Builder.Build());
            }

            [Fact]
            public void Builds_multiple_filters()
            {
                var filters = CreateMany<Filter>(2).ToList();

                Builder.AddFilters(filters.ToArray());

                Assert.Equal($"filter={filters[0].Expression},{filters[1].Expression}", Builder.Build());
            }
        }

        public class AppendQueryString : FilterBuilderTest
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
                var filter = Create<Filter>();

                Builder.AddFilter(filter);

                Assert.False(Builder.IsEmpty);

                Builder.AppendQueryString(MockQuery.Object);

                MockQuery.Verify(q => q.AddOrUpdate("filter", filter.Expression), Times.Once);
            }
        }
    }
}
