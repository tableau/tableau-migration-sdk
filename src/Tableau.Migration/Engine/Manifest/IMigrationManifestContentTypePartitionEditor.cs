using System;
using System.Collections.Generic;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Editable <see cref="IMigrationManifestContentTypePartition"/> interface for use during migration.
    /// </summary>
    public interface IMigrationManifestContentTypePartitionEditor : IMigrationManifestContentTypePartition
    {
        /// <summary>
        /// Gets the manifest entries keyed by source location.
        /// </summary>
        IReadOnlyDictionary<ContentLocation, IMigrationManifestEntryEditor> BySourceLocation { get; }

        /// <summary>
        /// Gets the manifest entries keyed by source ID.
        /// </summary>
        IReadOnlyDictionary<Guid, IMigrationManifestEntryEditor> BySourceId { get; }

        /// <summary>
        /// Gets the manifest entries keyed by source content URL.
        /// </summary>
        IReadOnlyDictionary<string, IMigrationManifestEntryEditor> BySourceContentUrl { get; }

        /// <summary>
        /// Gets the manifest entries keyed by mapped destination location.
        /// </summary>
        IReadOnlyDictionary<ContentLocation, IMigrationManifestEntryEditor> ByMappedLocation { get; }

        /// <summary>
        /// Gets the manifest entries keyed by destination ID.
        /// </summary>
        IReadOnlyDictionary<Guid, IMigrationManifestEntryEditor> ByDestinationId { get; }

        /// <summary>
        /// Creates entries from a set of entries to deep clone.
        /// </summary>
        /// <param name="entriesToCopy">The entries to clone.</param>
        /// <returns>The current partition, for fluent API usage.</returns>
        IMigrationManifestContentTypePartitionEditor CreateEntries(IReadOnlyCollection<IMigrationManifestEntry> entriesToCopy);

        /// <summary>
        /// Gets a <see cref="IMigrationManifestEntryBuilder"/> to use to create manifest entries.
        /// </summary>
        /// <param name="totalItemCount">The total number of expected manifest entries that will possibly be created.</param>
        /// <returns>The manifest entry builder.</returns>
        IMigrationManifestEntryBuilder GetEntryBuilder(int totalItemCount);
    }
}
