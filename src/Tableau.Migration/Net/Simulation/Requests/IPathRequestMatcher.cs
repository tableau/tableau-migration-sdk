using System;
using System.Net.Http;

namespace Tableau.Migration.Net.Simulation.Requests
{
    /// <summary>
    /// <see cref="IRequestMatcher"/> that matches on a HTTP method and request URL.
    /// </summary>
    public interface IPathRequestMatcher : IRequestMatcher
    {
        /// <summary>
        /// Gets the HTTP method to match on.
        /// </summary>
        HttpMethod Method { get; }

        /// <summary>
        /// Gets the request URL to match on.
        /// </summary>
        Uri RequestUrl { get; }
    }
}
