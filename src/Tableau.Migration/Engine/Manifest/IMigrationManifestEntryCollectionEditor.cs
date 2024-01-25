using System;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Interface for an editable migration manifest.
    /// </summary>
    public interface IMigrationManifestEntryCollectionEditor : IMigrationManifestEntryCollection
    {
        /// <summary>
        /// Creates a partition of manifest entries for a given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The created partition to add manifest entries to.</returns>
        IMigrationManifestContentTypePartitionEditor GetOrCreatePartition<TContent>();

        /// <summary>
        /// Creates a partition of manifest entries for a given content type.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <returns>The created partition to add manifest entries to.</returns>
        IMigrationManifestContentTypePartitionEditor GetOrCreatePartition(Type contentType);
    }
}
