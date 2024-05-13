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
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Net.Rest.Filtering;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Filtering
{
    public class FilterTests
    {
        public abstract class FilterTest : AutoFixtureTestBase
        { }

        public class Ctor : FilterTest
        {
            [Fact]
            public void Sets_Field()
            {
                var field = Create<string>();

                var filter = new Filter(field, Create<string>(), Create<object>());

                Assert.Equal(field, filter.Field);
            }

            [Fact]
            public void Sets_Operator()
            {
                var @operator = Create<string>();

                var filter = new Filter(Create<string>(), @operator, Create<object>());

                Assert.Equal(@operator, filter.Operator);
            }

            [Fact]
            public void Sets_Value()
            {
                var value = Create<string>();

                var filter = new Filter(Create<string>(), Create<string>(), value);

                Assert.Same(value, filter.Value);
            }

            [Fact]
            public void Sets_Values()
            {
                var values = CreateMany<string>(2).ToList();

                var filter = new Filter(Create<string>(), Create<string>(), values);

                var filterValue = Assert.IsAssignableFrom<IEnumerable<string>>(filter.Value);

                Assert.True(values.SequenceEqual(filterValue));
            }

            [Fact]
            public void Sets_Expression()
            {
                var filter = Create<Filter>();

                // Specifics are tested below
                Assert.NotEmpty(filter.Expression);
            }
        }

        public class Expression : FilterTest
        {
            private Filter CreateFilter(object value)
            {
                return new Filter(Create<string>(), Create<string>(), value);
            }

            private static void AssertExpression(Filter filter, string expectedExpressionValue)
            {
                Assert.Equal($"{filter.Field}:{filter.Operator}:{expectedExpressionValue}", filter.Expression);
            }

            [Fact]
            public void String_value()
            {
                var value = Create<string>();
                var filter = CreateFilter(value);

                AssertExpression(filter, value);
            }

            [Fact]
            public void Local_DateTime_value()
            {
                var value = DateTime.Now;
                var filter = CreateFilter(value);

                AssertExpression(filter, value.EnsureUtcKind().ToIso8601());
            }

            [Fact]
            public void Utc_DateTime_value()
            {
                var value = DateTime.UtcNow;
                var filter = CreateFilter(value);

                AssertExpression(filter, value.ToIso8601());
            }

            [Fact]
            public void Object_value()
            {
                var value = Create<Guid>();
                var filter = CreateFilter(value);

                AssertExpression(filter, value.ToString());
            }

            [Fact]
            public void Enumerable_String_value()
            {
                var values = CreateMany<string>(3);
                var filter = CreateFilter(values);

                AssertExpression(filter, $"[{string.Join(",", values)}]");
            }

            [Fact]
            public void Enumerable_DateTime_value()
            {
                var values = CreateMany<DateTime>(3);
                var filter = CreateFilter(values);

                AssertExpression(filter, $"[{string.Join(",", values.Select(v => v.EnsureUtcKind().ToIso8601()))}]");
            }

            [Fact]
            public void Enumerable_Object_value()
            {
                var values = CreateMany<Guid>(3);
                var filter = CreateFilter(values);

                AssertExpression(filter, $"[{string.Join(",", values.Select(v => v.ToString()))}]");
            }
        }

        public class EqualsOverride : FilterTest
        {
            [Fact]
            public void True()
            {
                var filter1 = Create<Filter>();
                var filter2 = new Filter(filter1.Field, filter1.Operator, filter1.Value);

                Assert.Equal(filter1.Expression, filter2.Expression);

                Assert.Equal(filter1, filter2);
            }

            [Fact]
            public void False()
            {
                var filter1 = Create<Filter>();
                var filter2 = Create<Filter>();

                Assert.NotEqual(filter1.Expression, filter2.Expression);

                Assert.NotEqual(filter1, filter2);
            }

            [Fact]
            public void False_when_case_differs()
            {
                var filter1 = Create<Filter>();
                var filter2 = new Filter(filter1.Field.ToUpper(), filter1.Operator, filter1.Value);

                Assert.Equal(filter1.Expression, filter2.Expression, ignoreCase: true);
                Assert.NotEqual(filter1.Expression, filter2.Expression);

                Assert.NotEqual(filter1, filter2);
            }
        }

        public class GetHashCodeOverride : FilterTest
        {
            [Fact]
            public void Returns_expression_hash_code()
            {
                var filter = Create<Filter>();

                var result = filter.GetHashCode();

                Assert.Equal(filter.Expression.GetHashCode(), result);
            }

            [Fact]
            public void Case_sensitive()
            {
                var filter1 = Create<Filter>();
                var filter2 = new Filter(filter1.Field.ToUpper(), filter1.Operator, filter1.Value);

                Assert.Equal(filter1.Expression, filter2.Expression, ignoreCase: true);
                Assert.NotEqual(filter1.Expression, filter2.Expression);

                var result1 = filter1.GetHashCode();
                var result2 = filter2.GetHashCode();

                Assert.NotEqual(result1, result2);
            }
        }
    }
}
