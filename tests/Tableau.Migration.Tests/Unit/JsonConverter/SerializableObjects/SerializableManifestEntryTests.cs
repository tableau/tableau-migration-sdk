//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Linq;
using Moq;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.JsonConverters.SerializableObjects;
using Xunit;

namespace Tableau.Migration.Tests.Unit.JsonConverter.SerializableObjects
{
    public class SerializableManifestEntryTests : AutoFixtureTestBase
    {
        private readonly Mock<IMigrationManifestEntryBuilder> mockPartition = new();

        [Fact]
        public void AsMigrationManifestEntryWithDestination()
        {
            var input = Create<SerializableManifestEntry>();

            var output = input.AsMigrationManifestEntry(mockPartition.Object);

            Assert.NotNull(output);

            Assert.Equal(input.Source!.AsContentReferenceStub(), output.Source);
            Assert.Equal(input.MappedLocation!.AsContentLocation(), output.MappedLocation);
            Assert.Equal(input.Status, output.Status.ToString());
            Assert.Equal(input.HasMigrated, output.HasMigrated);
            Assert.Equal(input.Destination?.AsContentReferenceStub(), output.Destination);
            Assert.Equal(input?.Errors?.Select(e => e.Error), output.Errors);
        }

        [Fact]
        public void AsMigrationManifestEntryWithoutDestination()
        {
            var input = Create<SerializableManifestEntry>();
            input.Destination = null;
            input.MappedLocation = input.Source!.Location;

            var output = input.AsMigrationManifestEntry(mockPartition.Object);

            Assert.NotNull(output);

            Assert.Equal(input.Source!.AsContentReferenceStub(), output.Source);
            Assert.Null(output.Destination);
            Assert.Equal(input.MappedLocation!.AsContentLocation(), output.MappedLocation);
            Assert.Equal(input.Status, output.Status.ToString());
            Assert.Equal(input.HasMigrated, output.HasMigrated);
            Assert.Equal(input?.Errors?.Select(e => e.Error), output.Errors);
        }

        [Theory]
        [InlineData(MigrationManifestEntryStatus.Migrated, null)]
        [InlineData(MigrationManifestEntryStatus.Skipped, false)]
        [InlineData(MigrationManifestEntryStatus.Skipped, true)]
        public void RoundtripCascadeSkip(MigrationManifestEntryStatus status, bool? cascadeSkip)
        {
            var mockEntry = Create<Mock<IMigrationManifestEntry>>();
            mockEntry.SetupGet(x => x.Status).Returns(status);
            mockEntry.SetupGet(x => x.CascadeSkip).Returns(cascadeSkip);

            var serialized = new SerializableManifestEntry(mockEntry.Object);

            var deserialized = serialized.AsMigrationManifestEntry(mockPartition.Object);

            Assert.Equal(mockEntry.Object.Status.ToString(), serialized.Status);
            Assert.Equal(mockEntry.Object.Status, deserialized.Status);

            Assert.Equal(mockEntry.Object.CascadeSkip, serialized.CascadeSkip);
            Assert.Equal(mockEntry.Object.CascadeSkip, deserialized.CascadeSkip);
        }

        [Fact]
        public void UpgradeCascadeSkip()
        {
            var input = Create<SerializableManifestEntry>();
            input.Status = MigrationManifestEntryStatus.Skipped.ToString();
            input.CascadeSkip = null;

            var deserialized = input.AsMigrationManifestEntry(mockPartition.Object);

            Assert.Equal(MigrationManifestEntryStatus.Skipped, deserialized.Status);
            Assert.False(deserialized.CascadeSkip);
        }

        [Fact]
        public void BadDeserialization_NullSource()
        {
            var input = Create<SerializableManifestEntry>();
            input.Source = null;

            Assert.Throws<ArgumentNullException>(() => input.AsMigrationManifestEntry(mockPartition.Object));
        }

        [Fact]
        public void BadDeserialization_NullMappedLocation()
        {
            var input = Create<SerializableManifestEntry>();
            input.MappedLocation = null;

            Assert.Throws<ArgumentNullException>(() => input.AsMigrationManifestEntry(mockPartition.Object));
        }

        [Fact]
        public void BadDeserialization_NullStatus()
        {
            var input = Create<SerializableManifestEntry>();
            input.Status = null;

            Assert.Throws<ArgumentNullException>(() => input.AsMigrationManifestEntry(mockPartition.Object));
        }
    }
}
