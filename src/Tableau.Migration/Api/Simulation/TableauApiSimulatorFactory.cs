using System;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Default <see cref="ITableauApiSimulatorFactory"/> implementation.
    /// </summary>
    public class TableauApiSimulatorFactory : ITableauApiSimulatorFactory
    {
        private readonly ITableauApiSimulatorCollection _simulators;
        private readonly IHttpContentSerializer _serializer;

        /// <summary>
        /// Creates a new <see cref="TableauApiSimulatorFactory"/> object.
        /// </summary>
        /// <param name="simulators">The simulator collection.</param>
        /// <param name="serializer">The serializer.</param>
        public TableauApiSimulatorFactory(ITableauApiSimulatorCollection simulators, IHttpContentSerializer serializer)
        {
            _simulators = simulators;
            _serializer = serializer;
        }

        /// <inheritdoc />
        public TableauApiSimulator GetOrCreate(Uri serverUrl)
        {
            var existing = _simulators.ForServer(serverUrl);
            if (existing is not null)
            {
                return existing;
            }

            var simulator = new TableauApiSimulator(serverUrl, _serializer);
            _simulators.AddOrUpdate(simulator);
            return simulator;
        }
    }
}
