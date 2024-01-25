using System.Net.Http;
using Tableau.Migration.Net.Simulation;

namespace Tableau.Migration.Api.Simulation
{
    internal sealed class TableauApiResponseSimulatorProvider : IResponseSimulatorProvider
    {
        private readonly ITableauApiSimulatorCollection _simulators;

        public TableauApiResponseSimulatorProvider(ITableauApiSimulatorCollection simulators)
        {
            _simulators = simulators;
        }

        /// <inheretdoc />
        public IResponseSimulator? ForRequest(HttpRequestMessage request)
            => _simulators.ForServer(request.RequestUri!)?.ResponseSimulator;
    }
}
