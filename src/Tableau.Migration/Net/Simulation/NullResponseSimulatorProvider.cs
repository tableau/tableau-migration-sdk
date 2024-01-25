using System.Net.Http;

namespace Tableau.Migration.Net.Simulation
{
    internal sealed class NullResponseSimulatorProvider : IResponseSimulatorProvider
    {
        public IResponseSimulator? ForRequest(HttpRequestMessage request)
            => null;
    }
}
