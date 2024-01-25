using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Simulation
{
    /// <summary>
    /// Interface for an object that can simulate a HTTP response.
    /// </summary>
    public interface IResponseSimulator
    {
        /// <summary>
        /// Creates a simulated response for the request.
        /// </summary>
        /// <param name="request">The request to respond to.</param>
        /// <param name="cancellationToken">A cancellation token to obey.</param>
        /// <returns>The simulated response message.</returns>
        Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
