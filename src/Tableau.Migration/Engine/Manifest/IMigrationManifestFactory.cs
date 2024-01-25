using System;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Interface for an object that can create <see cref="IMigrationManifest"/> objects.
    /// </summary>
    public interface IMigrationManifestFactory
    {
        /// <summary>
        /// Creates a new <see cref="IMigrationManifest"/> object.
        /// </summary>
        /// <param name="input">A migration input to use for initialization.</param>
        /// <param name="migrationId">The unique ID of the <see cref="IMigration"/> to include in the manifest.</param>
        /// <returns>The created <see cref="IMigrationManifestEditor"/> object.</returns>
        IMigrationManifestEditor Create(IMigrationInput input, Guid migrationId);

        /// <summary>
        /// Creates a new <see cref="IMigrationManifest"/> object.
        /// </summary>
        /// <param name="planId">The unique ID of the <see cref="IMigrationPlan"/> that the migration is running.</param>
        /// <param name="migrationId">The unique ID of the <see cref="IMigration"/> to include in the manifest.</param>
        /// <returns>The created <see cref="IMigrationManifestEditor"/> object.</returns>
        IMigrationManifestEditor Create(Guid planId, Guid migrationId);
    }
}
