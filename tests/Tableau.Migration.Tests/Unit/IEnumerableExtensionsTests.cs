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
using System.Collections;
using System.Linq;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class IEnumerableExtensionsTests
    {
        public abstract class IEnumerableExtensionsTest : AutoFixtureTestBase
        { }

        public class IsNullOrEmpty : IEnumerableExtensionsTest
        {
            [Fact]
            public void Returns_true_when_collection_is_null()
            {
                Assert.True(((IEnumerable?)null).IsNullOrEmpty());
            }

            [Fact]
            public void Returns_true_when_collection_is_empty()
            {
                Assert.True(Array.Empty<object>().IsNullOrEmpty());
            }

            [Fact]
            public void False()
            {
                Assert.False(new[] { 1 }.IsNullOrEmpty());
            }
        }

        public class ExceptNulls
        {
            public class ClassCollection : IEnumerableExtensionsTest
            {
                [Fact]
                public void Filters_nulls()
                {
                    var nonNullCount = 3;

                    var collection = CreateMany(nonNullCount, new object?[] { null }, new object?[] { null });

                    var result = collection.ExceptNulls();

                    Assert.True(result.Cast<object?>().SequenceEqual(collection.Where(i => i is not null)));
                }
            }

            public class StructCollection : IEnumerableExtensionsTest
            {
                [Fact]
                public void Filters_nulls()
                {
                    var nonNullCount = 3;

                    var collection = CreateMany(nonNullCount, new int?[] { null }, new int?[] { null });

                    var result = collection.ExceptNulls();

                    Assert.True(result.Cast<int?>().SequenceEqual(collection.Where(i => i is not null)));
                }
            }

            public class WithSelector : IEnumerableExtensionsTest
            {
                [Fact]
                public void Filters_nulls()
                {
                    var nonNullCount = 3;

                    var collection = CreateMany(nonNullCount, new object?[] { null }, new object?[] { null });

                    var result = collection.ExceptNulls(i => i);

                    Assert.True(result.Cast<object?>().SequenceEqual(collection.Where(i => i is not null)));
                }

                [Fact]
                public void Filters_projected_nulls()
                {
                    var collection = CreateMany<object>(3);

                    var result = collection.ExceptNulls(i => (object?)null);

                    Assert.Empty(result);
                }
            }
        }
    }
}
