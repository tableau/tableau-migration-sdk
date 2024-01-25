using System;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Interface for an object that contains the input given for a <see cref="IMigration"/>, 
    /// used to bootstrap migration engine dependency injection.
    /// </summary>
    /// <remarks>
    /// In almost all cases it is preferrable to inject the <see cref="IMigration"/> object, 
    /// this interface is only intended to be used to build <see cref="IMigration"/> object.
    /// </remarks>
    public interface IMigrationInput
    {
        /// <summary>
        /// Gets the unique identifier of the migration.
        /// This is generated as part of the input so that migration bootstrapping can avoid DI cycles.
        /// </summary>
        Guid MigrationId { get; }

        /// <summary>
        /// Gets the migration plan to execute.
        /// </summary>
        IMigrationPlan Plan { get; }

        /// <summary>
        /// Gets a manifest from a previous migration of the same plan to use to determine what progress has already been made.
        /// </summary>
        IMigrationManifest? PreviousManifest { get; }
    }
}
