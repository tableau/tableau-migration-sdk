using Moq;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.TestComponents.JsonConverters.JsonObjects;

namespace Tableau.Migration.TestComponents.Tests.JsonConverter.JsonObjects
{
    public class TestJsonManifestEntry : AutoFixtureTestBase
    {
        Mock<IMigrationManifestEntryBuilder> mockPartition = new();

        [Fact]
        public void AsMigrationManifestEntryWithDestination()
        {
            var input = Create<JsonManifestEntry>();

            var output = input.AsMigrationManifestEntry(mockPartition.Object);

            Assert.NotNull(output);

            Assert.Equal(input.Source!.AsContentReferenceStub(), output.Source);
            Assert.Equal(input.MappedLocation!.AsContentLocation(), output.MappedLocation);
            Assert.Equal((int)input.Status, (int)output.Status);
            Assert.Equal(input.HasMigrated, output.HasMigrated);

            //Destination location is reset on copy.
            Assert.Null(output.Destination);
        }

        [Fact]
        public void AsMigrationManifestEntryWithoutDestination()
        {
            var input = Create<JsonManifestEntry>();
            input.Destination = null;
            input.MappedLocation = input.Source!.Location;

            var output = input.AsMigrationManifestEntry(mockPartition.Object);

            Assert.NotNull(output);

            Assert.Equal(input.Source!.AsContentReferenceStub(), output.Source);
            Assert.Null(output.Destination);
            Assert.Equal(input.MappedLocation!.AsContentLocation(), output.MappedLocation);
            Assert.Equal((int)input.Status, (int)output.Status);
            Assert.Equal(input.HasMigrated, output.HasMigrated);
        }

        [Fact]
        public void BadDeserialization_NullSource()
        {
            var input = Create<JsonManifestEntry>();
            input.Source = null;

            Assert.Throws<ArgumentNullException>(() => input.AsMigrationManifestEntry(mockPartition.Object));
        }

        [Fact]
        public void BadDeserialization_NullMappedLocation()
        {
            var input = Create<JsonManifestEntry>();
            input.MappedLocation = null;

            Assert.Throws<ArgumentNullException>(() => input.AsMigrationManifestEntry(mockPartition.Object));
        }
    }
}
