using System;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Interface for an object that can create <see cref="TableauApiSimulator"/> objects.
    /// </summary>
    public interface ITableauApiSimulatorFactory
    {
        /// <summary>
        /// Creates an API simulator for the given server URL, or retrieves the existing simulator.
        /// </summary>
        /// <param name="serverUrl">The base server URL to get or create the API simulator for.</param>
        /// <returns>The API simulator.</returns>
        TableauApiSimulator GetOrCreate(Uri serverUrl);
    }
}
