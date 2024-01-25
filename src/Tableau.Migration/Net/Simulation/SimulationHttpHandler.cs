using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Simulation
{
    internal sealed class SimulationHttpHandler : DelegatingHandler
    {
        private readonly IResponseSimulatorProvider _responseSimulatorProvider;

        public SimulationHttpHandler(IResponseSimulatorProvider responseSimulatorProvider)
        {
            _responseSimulatorProvider = responseSimulatorProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseSimulator = _responseSimulatorProvider.ForRequest(request);
            if (responseSimulator is not null)
            {
                return await responseSimulator.RespondAsync(request, cancellationToken).ConfigureAwait(false);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // This shouldn't actually be called, it's here just in case so if anything does call it we don't try to access a real server.
            throw new NotImplementedException("Synchronous HTTP requests are not supported for simulation. Use an asynchronous request.");
        }
    }
}
