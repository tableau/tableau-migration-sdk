using System;
using System.Net.Http;

namespace Tableau.Migration.Net.Simulation.Requests
{
    /// <summary>
    /// Interface for an object that matches HTTP requests to simulate a response for.
    /// </summary>
    public interface IRequestMatcher : IEquatable<IRequestMatcher>
    {
        /// <summary>
        /// Determines whether the request matches this matcher's criteria.
        /// </summary>
        /// <param name="request">The request to attempt to match.</param>
        /// <returns>True if the request matches, otherwise false.</returns>
        bool Matches(HttpRequestMessage request);
    }
}
