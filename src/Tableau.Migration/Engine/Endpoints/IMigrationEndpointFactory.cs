using System;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that can create <see cref="IMigrationEndpoint"/>s from a migration plan.
    /// </summary>
    public interface IMigrationEndpointFactory
    {
        /// <summary>
        /// Creates the source endpoint for the given migration plan.
        /// </summary>
        /// <param name="plan">The migration plan to configure the endpoint for.</param>
        /// <returns>The created source endpoint.</returns>
        /// <exception cref="ArgumentException">If the source endpoint type is not supported by the factory.</exception>
        ISourceEndpoint CreateSource(IMigrationPlan plan);

        /// <summary>
        /// Creates the destination endpoint for the given migration plan.
        /// </summary>
        /// <param name="plan">The migration plan to configure the endpoint for.</param>
        /// <returns>The created destination endpoint.</returns>
        /// <exception cref="ArgumentException">If the source endpoint type is not supported by the factory.</exception>
        IDestinationEndpoint CreateDestination(IMigrationPlan plan);
    }
}
