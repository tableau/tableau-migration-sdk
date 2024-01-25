using System;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Interface for an object representing an in-progress migration. 
    /// This interface or its properties can be obtained through scoped dependency injection.
    /// </summary>
    public interface IMigration
    {
        /// <summary>
        /// Gets the unique ID of the migration run, generated each time a migration run starts.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the migration source endpoint to pull Tableau data from.
        /// </summary>
        ISourceEndpoint Source { get; }

        /// <summary>
        /// Gets the migration destination endpoint to push Tableau data to.
        /// </summary>
        IDestinationEndpoint Destination { get; }

        /// <summary>
        /// Gets the migration plan being run.
        /// </summary>
        IMigrationPlan Plan { get; }

        /// <summary>
        /// Gets the current migration manifest.
        /// </summary>
        IMigrationManifestEditor Manifest { get; }

        /// <summary>
        /// Gets the migration pipeline being executed.
        /// </summary>
        IMigrationPipeline Pipeline { get; }
    }
}
