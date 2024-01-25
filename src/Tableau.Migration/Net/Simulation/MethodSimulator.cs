using Tableau.Migration.Net.Simulation.Requests;
using Tableau.Migration.Net.Simulation.Responses;

namespace Tableau.Migration.Net.Simulation
{
    /// <summary>
    /// Interface for an object that simulates an API method and can produce simulated HTTP responses for a certain request path.
    /// </summary>
    /// <param name="RequestMatcher">The request matcher.</param>
    /// <param name="ResponseBuilder">The response builder.</param>
    public record MethodSimulator(IRequestMatcher RequestMatcher, IResponseBuilder ResponseBuilder)
    {
        /// <summary>
        /// Gets the override response builder.
        /// </summary>
        public IResponseBuilder? ResponseOverride { get; set; }

        /// <summary>
        /// Clears the current override response builder.
        /// </summary>
        /// <returns>The current method simulator, for fluent API usage.</returns>
        public MethodSimulator ClearResponseOverride()
        {
            ResponseOverride = null;
            return this;
        }

        /// <summary>
        /// Returns a string which represents the object instance.
        /// </summary>
        public override string ToString() => $"{nameof(RequestMatcher)}: {RequestMatcher}, {nameof(ResponseBuilder)}: {ResponseBuilder}, {nameof(ResponseOverride)}: {ResponseOverride?.ToString() ?? "<null>"}";
    }
}