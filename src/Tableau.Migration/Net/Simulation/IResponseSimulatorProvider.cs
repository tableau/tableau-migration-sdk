using System.Net.Http;

namespace Tableau.Migration.Net.Simulation
{
    /// <summary>
    /// Interface for an object that can provide response simulators for a given request.
    /// </summary>
    public interface IResponseSimulatorProvider
    {
        /// <summary>
        /// Gets a response simulator for the given request, or null.
        /// </summary>
        /// <param name="request">The request to get the response simulator for.</param>
        /// <returns>The response simulator, or null if there is no response simulator for the given request.</returns>
        IResponseSimulator? ForRequest(HttpRequestMessage request);
    }
}
