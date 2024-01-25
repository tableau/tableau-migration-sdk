using System.Net.Http;
using Polly;

namespace Tableau.Migration.Net.Policies
{
    /// <summary>
    /// Abstraction responsible to return all policies that apply for a given http request, giving to it resilience and transient-fault-handling.
    /// <see href="https://github.com/App-vNext/Polly"/>
    /// </summary>
    public interface IHttpPolicyWrapBuilder
    {
        /// <summary>
        /// Get all policies that apply for the http request. In case the request apply for more than one policy, 
        /// they must be wrapped by a <see href="https://github.com/App-vNext/Polly/wiki/PolicyWrap">PolicyWrap</see>
        /// </summary>
        /// <param name="httpRequest">The http request that we will request the policies</param>
        /// <returns>A async policy that apply to a given http response of a http request.</returns>
        IAsyncPolicy<HttpResponseMessage> GetRequestPolicies(HttpRequestMessage httpRequest);
    }
}
