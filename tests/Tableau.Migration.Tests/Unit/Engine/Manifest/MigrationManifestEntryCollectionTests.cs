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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class MigrationManifestEntryCollectionTests
    {
        #region - Test Classes -

        public class MigrationManifestEntryCollectionTest : AutoFixtureTestBase
        {
            protected readonly MigrationManifestEntryCollection Collection;

            public MigrationManifestEntryCollectionTest()
            {
                Collection = CreateEmpty();
            }

            protected MigrationManifestEntryCollection CreateEmpty() => new();
        }

        #endregion

        #region - Ctor -

        public sealed class Ctor : MigrationManifestEntryCollectionTest
        {
            [Fact]
            public void IntializesEmpty()
            {
                var c = new MigrationManifestEntryCollection();
                Assert.Empty(c);
            }

            [Fact]
            public void CopiesFromPreviousEntries()
            {
                var previousEntries = CreateMany<IMigrationManifestEntry>().ToImmutableArray();

                var mockPreviousCollection = Create<Mock<IMigrationManifestEntryCollection>>();
                mockPreviousCollection.Setup(x => x.CopyTo(It.IsAny<IMigrationManifestEntryCollectionEditor>()))
                    .Callback<IMigrationManifestEntryCollectionEditor>(e =>
                    {
                        e.GetOrCreatePartition(typeof(IWorkbook)).CreateEntries(previousEntries);
                    });

                var c = new MigrationManifestEntryCollection(mockPreviousCollection.Object);
                mockPreviousCollection.Verify(x => x.CopyTo(c), Times.Once);

                Assert.Equal(previousEntries.Length, c.Count());
                foreach (var entry in c)
                {
                    Assert.Equal(MigrationManifestEntryStatus.Pending, entry.Status);
                }
            }
        }

        #endregion

        #region - ForContentType -

        public class ForContentType : MigrationManifestEntryCollectionTest
        {
            [Fact]
            public void GenericType()
            {
                var p = Collection.GetOrCreatePartition<TestContentType>();
                p.CreateEntries(CreateMany<IMigrationManifestEntry>().ToImmutableArray());

                var result = Collection.ForContentType<TestContentType>();

                Assert.Same(p, result);
            }

            [Fact]
            public void NonGenericType()
            {
                var p = Collection.GetOrCreatePartition<TestContentType>();
                p.CreateEntries(CreateMany<IMigrationManifestEntry>().ToImmutableArray());

                var result = Collection.ForContentType(typeof(TestContentType));

                Assert.Same(p, result);
            }

            [Fact]
            public void LazyCreatesAndReusesPartitions()
            {
                var result1 = Collection.ForContentType<TestContentType>();
                var result2 = Collection.ForContentType<TestContentType>();

                var partition = Collection.GetOrCreatePartition<TestContentType>();

                Assert.Same(partition, result1);
                Assert.Same(partition, result2);
                Assert.Equal(typeof(TestContentType), result1.ContentType);
            }
        }

        #endregion

        #region - GetEnumerator -

        public class GetEnumerator : MigrationManifestEntryCollectionTest
        {
            private void TestExpectedSorting(Func<MigrationManifestEntryCollection, IEnumerator> getEnumerator)
            {
                var partition1 = Collection.GetOrCreatePartition<TestContentType>();
                partition1.CreateEntries(CreateMany<IMigrationManifestEntry>().ToImmutableArray());

                var partition2 = Collection.GetOrCreatePartition<OtherTestContentType>();
                partition2.CreateEntries(CreateMany<IMigrationManifestEntry>().ToImmutableArray());

                var expected = partition1.Concat(partition2);
                var actual = getEnumerator(Collection).ReadAll<IMigrationManifestEntry>();

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void OrderedByContentTypeAddOrderAndEntryAddOrder()
            {
                TestExpectedSorting(c => c.GetEnumerator());
            }

            [Fact]
            public void NonGeneric()
            {
                TestExpectedSorting(c => ((IEnumerable)c).GetEnumerator());
            }
        }

        #endregion

        #region - GetPartition -

        public class GetPartition : MigrationManifestEntryCollectionTest
        {
            [Fact]
            public void GenericOverload()
            {
                var p1 = Collection.GetOrCreatePartition<TestContentType>();
                var p2 = Collection.GetOrCreatePartition(typeof(TestContentType));

                Assert.Same(p1, p2);
                Assert.Equal(typeof(TestContentType), p1.ContentType);
            }

            [Fact]
            public void LazyCreatesAndReusesPartitions()
            {
                var p1 = Collection.GetOrCreatePartition<TestContentType>();
                var p2 = Collection.GetOrCreatePartition<TestContentType>();

                Assert.Same(p1, p2);
                Assert.Equal(typeof(TestContentType), p1.ContentType);
            }
        }

        #endregion

        #region - Equality - 
        public class Equality : MigrationManifestEntryCollectionTest
        {
            public Equality()
            { }

            private List<MigrationManifestEntry> CreateMigrationManifestEntries(int? count = 10)
            {
                List<MigrationManifestEntry> content = new();
                for (int i = 0; i < count; i++)
                {
                    content.Add(new Mock<MigrationManifestEntry>(MockEntryBuilder.Object, Create<ContentReferenceStub>()) { CallBase = true }.Object);
                }
                return content;
            }

            [Fact]
            public void Equal()
            {
                var c1 = CreateEmpty();
                var c2 = CreateEmpty();

                List<MigrationManifestEntry> content = CreateMigrationManifestEntries();

                c1.GetOrCreatePartition<TestContentType>().CreateEntries(content);
                c2.GetOrCreatePartition<TestContentType>().CreateEntries(content);

                Assert.True(c1.Equals(c1));

                Assert.True(c1.Equals(c2));
                Assert.True(c2.Equals(c1));

                Assert.True(c1 == c2);
                Assert.True(c2 == c1);

                Assert.False(c1 != c2);
                Assert.False(c2 != c1);
            }

            [Fact]
            public void SameEntriesDifferentTypesAreDifferent()
            {
                var c1 = CreateEmpty();
                var c2 = CreateEmpty();

                List<MigrationManifestEntry> content = CreateMigrationManifestEntries();

                c1.GetOrCreatePartition<TestContentType>().CreateEntries(content);
                c2.GetOrCreatePartition<OtherTestContentType>().CreateEntries(content);

                Assert.True(c1.Equals(c1));

                Assert.False(c1.Equals(c2));
                Assert.False(c2.Equals(c1));

                Assert.False(c1 == c2);
                Assert.False(c2 == c1);

                Assert.True(c1 != c2);
                Assert.True(c2 != c1);
            }

            [Fact]
            public void DifferentEntriesSameTypeAreDifferent()
            {
                var c1 = CreateEmpty();
                var c2 = CreateEmpty();

                var content1 = CreateMigrationManifestEntries();
                var content2 = CreateMigrationManifestEntries();

                Assert.False(content1.Equals(content2));

                c1.GetOrCreatePartition<TestContentType>().CreateEntries(content1);
                c2.GetOrCreatePartition<TestContentType>().CreateEntries(content2);

                Assert.True(c1.Equals(c1));

                Assert.False(c1.Equals(c2));
                Assert.False(c2.Equals(c1));

                Assert.True(c1 != c2);
                Assert.True(c2 != c1);
            }

            [Fact]
            public void DifferentEntriesDifferentTypeAreDifferent()
            {
                var c1 = CreateEmpty();
                var c2 = CreateEmpty();

                var content1 = CreateMigrationManifestEntries();
                var content2 = CreateMigrationManifestEntries();

                Assert.False(content1.Equals(content2));

                c1.GetOrCreatePartition<TestContentType>().CreateEntries(content1);
                c2.GetOrCreatePartition<OtherTestContentType>().CreateEntries(content2);

                Assert.True(c1.Equals(c1));

                Assert.False(c1.Equals(c2));
                Assert.False(c2.Equals(c1));

                Assert.True(c1 != c2);
                Assert.True(c2 != c1);
            }
        }

        #endregion
    }
}
