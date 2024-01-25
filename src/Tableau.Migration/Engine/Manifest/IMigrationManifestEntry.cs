using System;
using System.Collections.Generic;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Interface for an entry on a <see cref="IMigrationManifest"/> that describes the migration state of single content item.
    /// </summary>
    public interface IMigrationManifestEntry : IEquatable<IMigrationManifestEntry>
    {
        /// <summary>
        /// Gets the content item's source information.
        /// </summary>
        IContentReference Source { get; }

        /// <summary>
        /// Gets the content item's intended destination location, 
        /// regardless if a <see cref="Destination"/> value is available or not.
        /// This value should match the <see cref="Destination"/> value's location if available.
        /// </summary>
        ContentLocation MappedLocation { get; }

        /// <summary>
        /// Gets the content item's destination information, 
        /// or null if the content item was not migrated due to filtering,
        /// or otherwise not found in the destination during the course of the migration.
        /// </summary>
        IContentReference? Destination { get; }

        /// <summary>
        /// Gets the migration status of the content item for the current run.
        /// See <see cref="HasMigrated"/> for the migration status across all runs.
        /// </summary>
        MigrationManifestEntryStatus Status { get; }

        /// <summary>
        /// Gets whether or not the content item has been migrated, either in a previous run or the current run.
        /// </summary>
        bool HasMigrated { get; }

        /// <summary>
        /// Gets errors that occurred while migrating the content item.
        /// </summary>
        IReadOnlyList<Exception> Errors { get; }
    }
}
