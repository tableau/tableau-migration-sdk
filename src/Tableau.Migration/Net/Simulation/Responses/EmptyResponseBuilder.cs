using System.Net;

namespace Tableau.Migration.Net.Simulation.Responses
{
    /// <summary>
    /// <see cref="StaticStringResponseBuilder"/> that returns an empty content response.
    /// </summary>
    public class EmptyResponseBuilder : StaticStringResponseBuilder
    {
        /// <summary>
        /// Creates a new <see cref="EmptyResponseBuilder"/> object.
        /// </summary>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="requiresAuthentication">Whether the response requires an authenticated request.</param>
        public EmptyResponseBuilder(HttpStatusCode statusCode = HttpStatusCode.OK, bool requiresAuthentication = true)
            : base(null, statusCode, requiresAuthentication: requiresAuthentication)
        { }
    }
}
