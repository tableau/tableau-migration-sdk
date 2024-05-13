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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class StringEnumTests
    {
        private class TestStringEnum : StringEnum<TestStringEnum>
        {
            public const string Value3 = "Value3";
            public const string Value2 = "Value2";
            public const string Value1 = "Value1";
        }

        public abstract class StringEnumTest : AutoFixtureTestBase
        {
            protected static readonly ImmutableHashSet<Type> StringEnumTypes = typeof(StringEnum<>).Assembly
                .GetTypes()
                .Where(t => t.IsConcrete() && t.GetBaseTypes().Any(b => b.HasGenericTypeDefinition(typeof(StringEnum<>)) == true))
                .ToImmutableHashSet();

            protected static IImmutableList<string> GetValues(Type type)
            {
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(f => f.FieldType == typeof(string))
                    .OrderBy(f => f.MetadataToken);

                var values = ImmutableArray.CreateBuilder<string>();

                foreach (var field in fields)
                {
                    values.Add((string)field.GetValue(null)!);
                }

                return values.ToImmutable();
            }
        }

        public class GlobalTests : StringEnumTest
        {
            [Fact]
            public void All_values_are_unique()
            {
                Assert.NotEmpty(StringEnumTypes);

                foreach (var type in StringEnumTypes)
                {
                    var values = GetValues(type);

                    Assert.NotEmpty(values);

                    var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var value in values)
                        Assert.True(unique.Add(value));
                }
            }

            [Fact]
            public void Uses_correct_generic_argument()
            {
                foreach (var type in StringEnumTypes)
                    Assert.Contains(typeof(StringEnum<>).MakeGenericType(type), type.GetBaseTypes());
            }
        }

        public class GetAll
        {
            [Fact]
            public void Returns_all_values()
            {
                var expected = new[] { TestStringEnum.Value3, TestStringEnum.Value2, TestStringEnum.Value1 };
                var actual = TestStringEnum.GetAll();

                Assert.True(expected.SequenceEqual(actual));
            }

            public class WithExclusions
            {
                public class ArrayExclusions
                {
                    [Fact]
                    public void Excludes_specified_values()
                    {
                        var expected = new[] { TestStringEnum.Value2, TestStringEnum.Value1 };
                        var actual = TestStringEnum.GetAll(TestStringEnum.Value3);

                        Assert.True(expected.SequenceEqual(actual));
                    }
                }

                public class IEnumerableExclusions
                {
                    [Fact]
                    public void Excludes_specified_values()
                    {
                        var expected = new List<string> { TestStringEnum.Value2, TestStringEnum.Value1 };
                        var actual = TestStringEnum.GetAll(TestStringEnum.Value3);

                        Assert.True(expected.SequenceEqual(actual));
                    }
                }
            }
        }

        public class IsAMatch
        { 
            [Theory]
            [StringEnumData<TestStringEnum>]
            public void True(string value)
                => Assert.True(TestStringEnum.IsAMatch(value, value));

            [Theory]
            [InlineData(TestStringEnum.Value1, TestStringEnum.Value2)]
            [InlineData(TestStringEnum.Value2, TestStringEnum.Value1)]
            public void False(string first, string second)
                => Assert.False(TestStringEnum.IsAMatch(first, second));

            [Theory]
            [StringEnumData<TestStringEnum>]
            public void CaseInsensitiveMatch(string value)
            {
                Assert.True(TestStringEnum.IsAMatch(value, value.ToLower()));
                Assert.True(TestStringEnum.IsAMatch(value, value.ToUpper()));
                Assert.True(TestStringEnum.IsAMatch(value.ToLower(), value.ToUpper()));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void Handles_null_empty_and_whitespace(string? value)
            {
                Assert.True(TestStringEnum.IsAMatch(value, value));
            }
        }
    }
}
