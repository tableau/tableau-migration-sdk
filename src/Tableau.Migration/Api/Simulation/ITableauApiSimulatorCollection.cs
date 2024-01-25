using System;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Interface that contains <see cref="TableauApiSimulator"/>s registered to be used.
    /// </summary>
    public interface ITableauApiSimulatorCollection
    {
        /// <summary>
        /// Gets the API simulator for the given base URL.
        /// </summary>
        /// <param name="baseUrl">The base URL to get the API simulator for.</param>
        /// <returns>The API simulator, or null.</returns>
        TableauApiSimulator? ForServer(Uri baseUrl);

        /// <summary>
        /// Registers an API simulator to use by its base URL.
        /// </summary>
        /// <param name="simulator">The simulator to register.</param>
        void AddOrUpdate(TableauApiSimulator simulator);
    }
}
