using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Simulation.Responses
{
    /// <summary>
    /// Interface for an object that can build a HTTP response.
    /// </summary>
    public interface IResponseBuilder
    {
        /// <summary>
        /// Gets whether or not an unauthenticated response should be returned if the request is unauthenticated.
        /// </summary>
        bool RequiresAuthentication { get; }

        /// <summary>
        /// Builds a HTTP response for the given request.
        /// </summary>
        /// <param name="request">The HTTP request to respond to.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The HTTP response.</returns>
        Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancel);
    }
}
