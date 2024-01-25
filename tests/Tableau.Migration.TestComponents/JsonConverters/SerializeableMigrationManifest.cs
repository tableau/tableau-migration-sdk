using System.Collections.Immutable;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.TestComponents.JsonConverters
{
    public class SerializeableMigrationManifest
    {
        public Guid PlanId { get; set; }

        public Guid MigrationId { get; set; }

        public IReadOnlyList<Exception> Errors { get; set; } = new ImmutableArray<Exception>();

        public IMigrationManifestEntryCollectionEditor Entries { get; set; } = null!;

        public uint ManifestVersion { get; set; }
    }
}
