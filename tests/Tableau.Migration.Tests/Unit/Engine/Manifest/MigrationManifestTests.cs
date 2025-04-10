﻿//
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
using Moq;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class MigrationManifestTests
    {
        #region - Test Classes -

        public class MigrationManifestTest : AutoFixtureTestBase
        {
            protected readonly MigrationManifest Manifest;

            public MigrationManifestTest()
            {
                Manifest = new(PipelineProfile.ServerToCloud);
            }
        }

        #endregion

        #region - Ctor -

        public class Ctor : MigrationManifestTest
        {
            [Fact]
            public void NoPreviousManifest()
            {
                var planId = Guid.NewGuid();
                var migrationId = Guid.NewGuid();

                var m = new MigrationManifest(planId, migrationId, PipelineProfile.ServerToCloud);

                Assert.Equal(planId, m.PlanId);
                Assert.Equal(migrationId, m.MigrationId);

                Assert.Same(((IMigrationManifest)m).Entries, m.Entries);
                Assert.Empty(m.Entries);
            }

            [Fact]
            public void PreviousManifest()
            {
                var planId = Guid.NewGuid();
                var migrationId = Guid.NewGuid();

                var mockPreviousManifest = Create<Mock<IMigrationManifest>>();
                mockPreviousManifest.Setup(x => x.PipelineProfile).Returns(PipelineProfile.ServerToCloud);

                var m = new MigrationManifest(planId, migrationId, PipelineProfile.ServerToCloud, mockPreviousManifest.Object);

                Assert.Equal(planId, m.PlanId);
                Assert.Equal(migrationId, m.MigrationId);

                Assert.Same(((IMigrationManifest)m).Entries, m.Entries);
                mockPreviousManifest.Verify(x => x.Entries.CopyTo(m.Entries), Times.Once);
            }
        }

        #endregion

        #region - Entries -

        public class EntryBuilder : MigrationManifestTest
        {
            [Fact]
            public void SameEntryCollection()
            {
                Assert.Same(((IMigrationManifest)Manifest).Entries, Manifest.Entries);
            }
        }

        #endregion

        #region - AddErrors -

        public class AddErrors : MigrationManifestTest
        {
            [Fact]
            public void AddSingleError()
            {
                var exception = new Exception();

                var result = Manifest.AddErrors(exception);

                Assert.Same(result, Manifest);
                var resultItem = Assert.Single(Manifest.Errors);
                Assert.Same(exception, resultItem);
            }

            [Fact]
            public void AddMultipleErrors()
            {
                var exceptions = new[] { new Exception(), new Exception() };

                var result = Manifest.AddErrors(exceptions);

                Assert.Same(result, Manifest);
                Assert.Equal(2, Manifest.Errors.Count);
                Assert.Contains(exceptions[0], Manifest.Errors);
                Assert.Contains(exceptions[1], Manifest.Errors);
            }
        }

        #endregion

        #region - Equality -

        public class Equality : MigrationManifestTest
        {
            [Fact]
            public void Equal()
            {
                var planId = Guid.NewGuid();
                var migrationId = Guid.NewGuid();

                var mockEntryCollection = new Mock<MigrationManifestEntryCollection>(null);
                mockEntryCollection.Setup(c => c.Equals(It.IsAny<IMigrationManifestEntryCollection?>())).Returns(true);

                var mockManifest1 = new Mock<MigrationManifest>(planId, migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest1.Setup(m => m.Entries).Returns(mockEntryCollection.Object);

                var mockManifest2 = new Mock<MigrationManifest>(planId, migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest2.Setup(m => m.Entries).Returns(mockEntryCollection.Object);


                Assert.True(mockManifest1.Object.Equals(mockManifest1.Object));

                Assert.True(mockManifest1.Object.Equals(mockManifest2.Object));
                Assert.True(mockManifest2.Object.Equals(mockManifest1.Object));

                Assert.True(mockManifest1.Object == mockManifest2.Object);
                Assert.True(mockManifest2.Object == mockManifest1.Object);

                Assert.False(mockManifest1.Object != mockManifest2.Object);
                Assert.False(mockManifest2.Object != mockManifest1.Object);
            }

            [Fact]
            public void VersionsAreDifferent()
            {
                var planId = Guid.NewGuid();
                var migrationId = Guid.NewGuid();

                var mockEntryCollection = new Mock<MigrationManifestEntryCollection>(null);
                mockEntryCollection.Setup(c => c.Equals(It.IsAny<IMigrationManifestEntryCollection?>())).Returns(true);

                var mockManifest1 = new Mock<MigrationManifest>(planId, migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest1.Setup(m => m.Entries).Returns(mockEntryCollection.Object);
                mockManifest1.Setup(m => m.ManifestVersion).Returns(1);

                var mockManifest2 = new Mock<MigrationManifest>(planId, migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest2.Setup(m => m.Entries).Returns(mockEntryCollection.Object);
                mockManifest1.Setup(m => m.ManifestVersion).Returns(2);

                Assert.False(mockManifest1.Object.Equals(mockManifest2.Object));
                Assert.False(mockManifest2.Object.Equals(mockManifest1.Object));

                Assert.False(mockManifest1.Object == mockManifest2.Object);
                Assert.False(mockManifest2.Object == mockManifest1.Object);

                Assert.True(mockManifest1.Object != mockManifest2.Object);
                Assert.True(mockManifest2.Object != mockManifest1.Object);
            }

            [Fact]
            public void EntriesAreDifferent()
            {
                var planId = Guid.NewGuid();
                var migrationId = Guid.NewGuid();

                var mockEntryCollection = new Mock<MigrationManifestEntryCollection>(null);
                mockEntryCollection.Setup(c => c.Equals(It.IsAny<IMigrationManifestEntryCollection?>())).Returns(false);

                var mockManifest1 = new Mock<MigrationManifest>(planId, migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest1.Setup(m => m.Entries).Returns(mockEntryCollection.Object);

                var mockManifest2 = new Mock<MigrationManifest>(planId, migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest2.Setup(m => m.Entries).Returns(mockEntryCollection.Object);

                Assert.False(mockManifest1.Object.Equals(mockManifest2.Object));
                Assert.False(mockManifest2.Object.Equals(mockManifest1.Object));

                Assert.False(mockManifest1.Object == mockManifest2.Object);
                Assert.False(mockManifest2.Object == mockManifest1.Object);

                Assert.True(mockManifest1.Object != mockManifest2.Object);
                Assert.True(mockManifest2.Object != mockManifest1.Object);

            }

            [Fact]
            public void PlanIdIsDifferent()
            {
                var migrationId = Guid.NewGuid();

                var mockEntryCollection = new Mock<MigrationManifestEntryCollection>(null);
                mockEntryCollection.Setup(c => c.Equals(It.IsAny<IMigrationManifestEntryCollection?>())).Returns(true);

                var mockManifest1 = new Mock<MigrationManifest>(Guid.NewGuid(), migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest1.Setup(m => m.Entries).Returns(mockEntryCollection.Object);

                var mockManifest2 = new Mock<MigrationManifest>(Guid.NewGuid(), migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest2.Setup(m => m.Entries).Returns(mockEntryCollection.Object);


                Assert.True(mockManifest1.Object.Equals(mockManifest1.Object));

                Assert.False(mockManifest1.Object.Equals(mockManifest2.Object));
                Assert.False(mockManifest2.Object.Equals(mockManifest1.Object));

                Assert.False(mockManifest1.Object == mockManifest2.Object);
                Assert.False(mockManifest2.Object == mockManifest1.Object);

                Assert.True(mockManifest1.Object != mockManifest2.Object);
                Assert.True(mockManifest2.Object != mockManifest1.Object);

            }

            [Fact]
            public void MigrationIdIsDifferent()
            {
                var planId = Guid.NewGuid();

                var mockEntryCollection = new Mock<MigrationManifestEntryCollection>(null);
                mockEntryCollection.Setup(c => c.Equals(It.IsAny<IMigrationManifestEntryCollection?>())).Returns(true);

                var mockManifest1 = new Mock<MigrationManifest>(planId, Guid.NewGuid(), PipelineProfile.ServerToCloud, null);
                mockManifest1.Setup(m => m.Entries).Returns(mockEntryCollection.Object);

                var mockManifest2 = new Mock<MigrationManifest>(planId, Guid.NewGuid(), PipelineProfile.ServerToCloud, null);
                mockManifest2.Setup(m => m.Entries).Returns(mockEntryCollection.Object);


                Assert.True(mockManifest1.Object.Equals(mockManifest1.Object));

                Assert.False(mockManifest1.Object.Equals(mockManifest2.Object));
                Assert.False(mockManifest2.Object.Equals(mockManifest1.Object));

                Assert.False(mockManifest1.Object == mockManifest2.Object);
                Assert.False(mockManifest2.Object == mockManifest1.Object);

                Assert.True(mockManifest1.Object != mockManifest2.Object);
                Assert.True(mockManifest2.Object != mockManifest1.Object);
            }

            [Fact]
            public void PipelineProfileIsDifferent()
            {
                var planId = Guid.NewGuid();
                var migrationId = Guid.NewGuid();

                var mockEntryCollection = new Mock<MigrationManifestEntryCollection>(null);
                mockEntryCollection.Setup(c => c.Equals(It.IsAny<IMigrationManifestEntryCollection?>())).Returns(true);

                var mockManifest1 = new Mock<MigrationManifest>(planId, migrationId, PipelineProfile.ServerToCloud, null);
                mockManifest1.Setup(m => m.Entries).Returns(mockEntryCollection.Object);

                var mockManifest2 = new Mock<MigrationManifest>(planId, migrationId, PipelineProfile.ServerToServer, null);
                mockManifest2.Setup(m => m.Entries).Returns(mockEntryCollection.Object);


                Assert.True(mockManifest1.Object.Equals(mockManifest1.Object));

                Assert.False(mockManifest1.Object.Equals(mockManifest2.Object));
                Assert.False(mockManifest2.Object.Equals(mockManifest1.Object));

                Assert.False(mockManifest1.Object == mockManifest2.Object);
                Assert.False(mockManifest2.Object == mockManifest1.Object);

                Assert.True(mockManifest1.Object != mockManifest2.Object);
                Assert.True(mockManifest2.Object != mockManifest1.Object);
            }
        }

        #endregion
    }
}
