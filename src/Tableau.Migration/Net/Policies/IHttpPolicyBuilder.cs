using System.Net.Http;
using Polly;

namespace Tableau.Migration.Net.Policies
{
    /// <summary>
    /// Abstraction build a policy that apply for a given http request.
    /// <see href="https://github.com/App-vNext/Polly"/>
    /// </summary>
    public interface IHttpPolicyBuilder
    {
        /// <summary>
        /// Build and return the policy that apply for the http request.
        /// </summary>
        /// <param name="httpRequest">The http request that we will request the policies</param>
        /// <returns>A async policy that apply to a given http response of a http request.</returns>
        IAsyncPolicy<HttpResponseMessage>? Build(HttpRequestMessage httpRequest);
    }
}
