using System;
using System.Collections.Concurrent;
using Tableau.Migration.Net.Simulation;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Default <see cref="ITableauApiSimulatorCollection"/> implementation.
    /// </summary>
    public class TableauApiSimulatorCollection : ITableauApiSimulatorCollection
    {
        private readonly ConcurrentDictionary<Uri, TableauApiSimulator> _simulators = new(BaseUrlComparer.Instance);

        /// <inheritdoc />
        public TableauApiSimulator? ForServer(Uri baseUrl)
        {
            if (_simulators.TryGetValue(baseUrl, out var simulator))
            {
                return simulator;
            }

            return null;
        }

        /// <inheritdoc />
        public void AddOrUpdate(TableauApiSimulator simulator)
            => _simulators.AddOrUpdate(simulator.ServerUrl, simulator, (k, v) => simulator);
    }
}
