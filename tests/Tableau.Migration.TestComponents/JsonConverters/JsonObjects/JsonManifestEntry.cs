using CommunityToolkit.Diagnostics;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.TestComponents.JsonConverters.JsonObjects
{
    public class JsonManifestEntry : IMigrationManifestEntry
    {
        public JsonContentReference? Source { get; set; }
        public JsonContentLocation? MappedLocation { get; set; }
        public JsonContentReference? Destination { get; set; }
        public int Status { get; set; }
        public bool HasMigrated { get; set; }

        IContentReference IMigrationManifestEntry.Source => Source!.AsContentReferenceStub();

        ContentLocation IMigrationManifestEntry.MappedLocation => MappedLocation!.AsContentLocation();

        IContentReference? IMigrationManifestEntry.Destination => Destination?.AsContentReferenceStub();

        MigrationManifestEntryStatus IMigrationManifestEntry.Status => (MigrationManifestEntryStatus)Status;

        bool IMigrationManifestEntry.HasMigrated => HasMigrated;

        IReadOnlyList<Exception> IMigrationManifestEntry.Errors => Array.Empty<Exception>();

        /// <summary>
        /// Throw exception if any values are still null
        /// </summary>
        public void VerifyDeseralization()
        {
            // Destination can be null, so we shouldn't do a nullability check on it

            Guard.IsNotNull(Source, nameof(Source));
            Guard.IsNotNull(MappedLocation, nameof(MappedLocation));

            Source.VerifyDeseralization();
            MappedLocation.VerifyDeseralization();
        }

        /// <summary>
        /// Returns current object as a <see cref="IMigrationManifestEntry"/>
        /// </summary>
        /// <returns></returns>
        public IMigrationManifestEntry AsMigrationManifestEntry(IMigrationManifestEntryBuilder partition)
        {
            VerifyDeseralization();
            var ret = new MigrationManifestEntry(partition, this);

            return ret;
        }

        public bool Equals(IMigrationManifestEntry? other)
            => MigrationManifestEntry.Equals(this, other);
    }
}