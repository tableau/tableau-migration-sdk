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
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class InheritedTypeComparerTests
    {
        private class BaseType
        { }

        private class DerivedType1 : BaseType
        { }

        private class DerivedType2 : BaseType
        { }

        private class OtherType
        { }

        public abstract class InheritedTypeComparerTest
        {
            internal readonly InheritedTypeComparer Comparer = new();
        }

        public class Equality : InheritedTypeComparerTest
        {
            [Theory]
            [InlineData(typeof(BaseType), typeof(BaseType))]
            [InlineData(typeof(DerivedType1), typeof(BaseType))]
            [InlineData(typeof(DerivedType2), typeof(BaseType))]
            [InlineData(typeof(BaseType), typeof(DerivedType1))]
            [InlineData(typeof(BaseType), typeof(DerivedType2))]
            public void True(Type type1, Type type2)
            {
                AssertEquality(type1, type2, true);

                AssertCollectionComparison(type1, true, type2);
                AssertCollectionComparison(type2, true, type1);
            }

            [Theory]
            [InlineData(typeof(DerivedType1), typeof(DerivedType2))]
            public void False(Type type1, Type type2)
            {
                AssertEquality(type1, type2, false);

                AssertCollectionComparison(type2, false, type1);
                AssertCollectionComparison(type1, false, type2);
            }

            private void AssertEquality(Type type1, Type type2, bool expectedEquality)
                => Assert.Equal(expectedEquality, Comparer.Equals(type1, type2));

            private void AssertCollectionComparison(Type comparisonType, bool expectedContains, Type collectionValue)
                => AssertCollectionComparison(comparisonType, expectedContains, new[] { collectionValue });

            private void AssertCollectionComparison(Type comparisonType, bool expectedContains, IEnumerable<Type> collectionValues)
            {
                var distinctCollectionValues = collectionValues.Distinct(Comparer);

                var dictionary = distinctCollectionValues.ToDictionary(k => k, k => false, Comparer);

                foreach (var value in collectionValues)
                {
                    Assert.Contains(value, dictionary);
                }

                Assert.Equal(expectedContains, dictionary.ContainsKey(comparisonType));

                var immutableDictionary = distinctCollectionValues.ToDictionary(k => k, k => false, Comparer).ToImmutableDictionary(Comparer);

                foreach (var value in collectionValues)
                {
                    Assert.True(immutableDictionary.ContainsKey(value));
                }

                Assert.Equal(expectedContains, immutableDictionary.ContainsKey(comparisonType));

                var hashSet = new HashSet<Type>(distinctCollectionValues, Comparer);

                foreach (var value in collectionValues)
                {
                    Assert.Contains(value, hashSet);
                }

                Assert.Equal(expectedContains, hashSet.Contains(comparisonType));
            }
        }
    }
}
