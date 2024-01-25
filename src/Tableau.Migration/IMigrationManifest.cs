using System;
using System.Collections.Generic;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that describes the various Tableau data items found to migrate and their migration results.
    /// </summary>
    public interface IMigrationManifest : IEquatable<IMigrationManifest>
    {
        /// <summary>
        /// Gets the unique identifier of the <see cref="IMigrationPlan"/> that was executed to produce this manifest.
        /// </summary>
        Guid PlanId { get; }

        /// <summary>
        /// Gets the unique identifier of the migration run that produced this manifest.
        /// </summary>
        Guid MigrationId { get; }

        /// <summary>
        /// Gets top-level errors that are not related to any Tableau content item but occurred during the migration.
        /// </summary>
        IReadOnlyList<Exception> Errors { get; }

        /// <summary>
        /// Gets the collection of manifest entries.
        /// </summary>
        IMigrationManifestEntryCollection Entries { get; }

        /// <summary>
        /// Gets the version of this manifest. Used for serialization.
        /// </summary>
        uint ManifestVersion { get; }
    }
}
