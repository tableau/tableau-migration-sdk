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
using AutoFixture;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class MigrationManifestEntryTests
    {
        public class MigrationManifestEntryTest : AutoFixtureTestBase
        {
            public MigrationManifestEntryTest()
            {
                AutoFixture.Register(() =>
                {
                    return new Exception(Create<string>());
                });

                AutoFixture.Register(() =>
                {
                    var ret = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());
                    ret.SetFailed(FixtureFactory.CreateErrors(AutoFixture));
                    return ret;
                });
            }
        }

        #region - Ctor -

        public class Ctor : MigrationManifestEntryTest
        {
            IMigrationManifestEntry CreateManifestEntry()
            {
                var errors = new List<Exception>();
                errors.Add(new Exception("Test Error"));

                var ret = new Mock<IMigrationManifestEntry>();
                ret.Setup(x => x.Source).Returns(Create<IContentReference>());
                ret.Setup(x => x.Destination).Returns(Create<IContentReference>());
                ret.Setup(x => x.MappedLocation).Returns(Create<ContentLocation>());
                ret.Setup(x => x.Status).Returns(MigrationManifestEntryStatus.Migrated);
                ret.Setup(x => x.Errors).Returns(new List<Exception>(errors));

                return ret.Object;
            }

            [Fact]
            public void FromSourceReference()
            {
                var sourceRef = Create<ContentReferenceStub>();

                var e = new MigrationManifestEntry(MockEntryBuilder.Object, sourceRef);

                Assert.Same(sourceRef, e.Source);
                Assert.Equal(e.Source.Location, e.MappedLocation);
                Assert.Null(e.Destination);
                Assert.Equal(MigrationManifestEntryStatus.Pending, e.Status);
                Assert.Empty(e.Errors);
            }

            [Fact]
            public void FromPreviousMigration()
            {
                var previousEntry = CreateManifestEntry();

                var e = new MigrationManifestEntry(MockEntryBuilder.Object, previousEntry);

                Assert.Same(previousEntry.Source, e.Source);
                Assert.Equal(previousEntry.MappedLocation, e.MappedLocation);
                Assert.Equal(previousEntry.Status, e.Status);
                Assert.Equal(previousEntry.HasMigrated, e.HasMigrated);
                Assert.Equal(previousEntry.Destination, e.Destination);
                Assert.Equal(previousEntry.Errors, e.Errors);
            }

            [Fact]
            public void FromUpdatedPreviousMigration()
            {
                var previousEntry = CreateManifestEntry();

                var sourceRef = Create<ContentReferenceStub>();

                var e = new MigrationManifestEntry(MockEntryBuilder.Object, previousEntry, sourceRef);

                Assert.Same(sourceRef, e.Source);
                Assert.Equal(previousEntry.MappedLocation, e.MappedLocation);
                Assert.Equal(previousEntry.Status, e.Status);
                Assert.Equal(previousEntry.HasMigrated, e.HasMigrated);
                Assert.Equal(previousEntry.Destination, e.Destination);
                Assert.Equal(previousEntry.Errors, e.Errors);
            }
        }

        #endregion

        #region - MapToDestination -

        public class MapToDestination : MigrationManifestEntryTest
        {
            [Fact]
            public void MapsToDestination()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var destination = Create<ContentLocation>();
                e.MapToDestination(destination);

                Assert.Equal(destination, e.MappedLocation);
            }

            [Fact]
            public void ClearsDestinationInfoWithoutMatch()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var destination = Create<IContentReference>();
                e.DestinationFound(destination);

                var newMappedLocation = Create<ContentLocation>();
                e.MapToDestination(newMappedLocation);

                Assert.Equal(newMappedLocation, e.MappedLocation);
                Assert.Null(e.Destination);

                MockEntryBuilder.Verify(x => x.DestinationInfoUpdated(e, destination), Times.Once);
            }

            [Fact]
            public void RetainsDestinationInfoWithMatch()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var destination = Create<IContentReference>();
                e.DestinationFound(destination);

                e.MapToDestination(destination.Location);

                Assert.Equal(destination.Location, e.MappedLocation);
                Assert.Same(destination, e.Destination);

                MockEntryBuilder.Verify(x => x.DestinationInfoUpdated(
                    It.IsAny<IMigrationManifestEntryEditor>(), It.IsAny<IContentReference?>()),
                    Times.Once);
            }
        }

        #endregion

        #region - DestinationFound -

        public class DestinationFound : MigrationManifestEntryTest
        {
            [Fact]
            public void SetsDestinationInfoAndMappedLocation()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var destination = Create<IContentReference>();
                e.DestinationFound(destination);

                Assert.Same(destination, e.Destination);
                Assert.Equal(destination.Location, e.MappedLocation);

                MockEntryBuilder.Verify(x => x.DestinationInfoUpdated(e, null), Times.Once);
            }

            [Fact]
            public void NotifiesOnUpdate()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var oldDestination = Create<IContentReference>();
                e.DestinationFound(oldDestination);

                var newDestination = Create<IContentReference>();
                e.DestinationFound(newDestination);

                Assert.Same(newDestination, e.Destination);
                Assert.Equal(newDestination.Location, e.MappedLocation);

                MockEntryBuilder.Verify(x => x.DestinationInfoUpdated(e, oldDestination), Times.Once);
                MockEntryBuilder.Verify(x => x.DestinationInfoUpdated(
                    It.IsAny<IMigrationManifestEntryEditor>(), It.IsAny<IContentReference?>()),
                    Times.Exactly(2));
            }
        }

        #endregion

        #region - SetSkipped -

        public class SetSkipped : MigrationManifestEntryTest
        {
            [Fact]
            public void SetsStatus()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var e2 = e.SetSkipped();

                Assert.Same(e, e2);
                Assert.Equal(MigrationManifestEntryStatus.Skipped, e.Status);
            }
        }

        #endregion

        #region - SetMigrated -

        public class SetMigrated : MigrationManifestEntryTest
        {
            [Fact]
            public void SetsStatusAndHasMigratedFlag()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var e2 = e.SetMigrated();

                Assert.Same(e, e2);
                Assert.Equal(MigrationManifestEntryStatus.Migrated, e.Status);
                Assert.True(e.HasMigrated);
            }
        }

        #endregion

        #region - SetCanceled -

        public class SetCanceled : MigrationManifestEntryTest
        {
            [Fact]
            public void SetsStatus()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var e2 = e.SetCanceled();

                Assert.Same(e, e2);
                Assert.Equal(MigrationManifestEntryStatus.Canceled, e.Status);
            }
        }

        #endregion

        #region - SetFailed -

        public class SetFailed : MigrationManifestEntryTest
        {
            [Fact]
            public void SetsStatusAndErrors()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var errors = CreateMany<Exception>();
                var e2 = e.SetFailed(errors);

                Assert.Same(e, e2);
                Assert.Equal(MigrationManifestEntryStatus.Error, e.Status);
                Assert.Equal(errors, e.Errors);
            }

            [Fact]
            public void SingleErrorParams()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                var error = Create<Exception>();
                var e2 = e.SetFailed(error);

                Assert.Same(e, e2);
                Assert.Equal(MigrationManifestEntryStatus.Error, e.Status);
                Assert.Equal(new[] { error }, e.Errors);
            }

            [Fact]
            public void EmptyErrorParams()
            {
                var e = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());
                var e2 = e.SetFailed();

                Assert.Same(e, e2);
                Assert.Equal(MigrationManifestEntryStatus.Error, e.Status);
                Assert.Empty(e.Errors);
            }
        }

        #endregion

        #region - Equality -

        public class Equality : MigrationManifestEntryTest
        {
            private readonly ContentReferenceStub BaseSource;

            public Equality()
            {
                BaseSource = Create<ContentReferenceStub>();
            }

            [Fact]
            public void EqualFromSource()
            {
                MigrationManifestEntry e1 = new(MockEntryBuilder.Object, BaseSource);
                MigrationManifestEntry e2 = new(MockEntryBuilder.Object, BaseSource);

                Assert.NotNull(e1);
                Assert.NotNull(e2);

                Assert.True(e1.Equals(e1));

                Assert.True(e1.Equals(e2));
                Assert.True(e2.Equals(e1));

                Assert.True(e1 == e2);
                Assert.True(e2 == e1);
            }

            [Fact]
            public void EqualFromCopy()
            {
                MigrationManifestEntry e1 = new(MockEntryBuilder.Object, BaseSource);
                MigrationManifestEntry e2 = new(MockEntryBuilder.Object, e1);

                Assert.NotNull(e1);
                Assert.NotNull(e2);

                Assert.True(e1.Equals(e1));

                Assert.True(e1.Equals(e2));
                Assert.True(e2.Equals(e1));

                Assert.True(e1 == e2);
                Assert.True(e2 == e1);
            }

            [Fact]
            public void Nullability()
            {
                var stub = Create<ContentReferenceStub>();

                var e1 = new MigrationManifestEntry(MockEntryBuilder.Object, stub);

                Assert.False(e1.Equals(null));

                Assert.False(e1 == null);
                Assert.False(null == e1);

                Assert.True(e1 != null);
                Assert.True(null != e1);
            }

            [Fact]
            public void DifferentSource()
            {
                var e1 = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());
                var e2 = new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>());

                Assert.False(e1.Equals(e2));
                Assert.False(e2.Equals(e1));

                Assert.False(e1 == e2);
                Assert.True(e1 != e2);
            }

            [Fact]
            public void DifferentStatus()
            {
                var e1 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);
                var e2 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);

                e2.SetCanceled();

                Assert.False(e1.Equals(e2));
                Assert.False(e2.Equals(e1));

                Assert.False(e1 == e2);
                Assert.True(e1 != e2);
            }

            [Fact]
            public void DestinationDifferent()
            {
                var e1 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);
                var e2 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);

                e1.DestinationFound(Create<IContentReference>());
                e1.DestinationFound(Create<IContentReference>());

                Assert.False(e1.Equals(e2));
                Assert.False(e2.Equals(e1));

                Assert.False(e1 == e2);
                Assert.True(e1 != e2);
            }

            [Fact]
            public void DestinationNull()
            {
                var e1 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);
                var e2 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);

                e1.DestinationFound(Create<IContentReference>());

                Assert.False(e1.Equals(e2));
                Assert.False(e2.Equals(e1));

                Assert.False(e1 == e2);
                Assert.True(e1 != e2);
            }

            [Fact]
            public void ErrorsDifferent()
            {
                var e1 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);
                var e2 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);

                e1.SetFailed(CreateMany<Exception>(5).ToList());
                e2.SetFailed(CreateMany<Exception>(5).ToList());

                Assert.False(e1.Equals(e2));
                Assert.False(e2.Equals(e1));

                Assert.False(e1 == e2);
                Assert.True(e1 != e2);
            }

            [Fact]
            public void ErrorsNull()
            {
                var e1 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);
                var e2 = new MigrationManifestEntry(MockEntryBuilder.Object, BaseSource);

                e1.SetFailed(CreateMany<Exception>(5).ToList());

                Assert.False(e1.Equals(e2));
                Assert.False(e2.Equals(e1));

                Assert.False(e1 == e2);
                Assert.True(e1 != e2);
            }
        }

        #endregion
    }
}