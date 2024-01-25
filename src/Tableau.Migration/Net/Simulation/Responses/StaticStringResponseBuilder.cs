using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Simulation.Responses
{
    /// <summary>
    /// <see cref="IResponseBuilder"/> implementation that returns a static response.
    /// </summary>
    public class StaticStringResponseBuilder : IResponseBuilder
    {
        private readonly string? _response;
        private readonly HttpStatusCode _statusCode;
        private readonly string _contentType;

        /// <inheritdoc />
        public bool RequiresAuthentication { get; }

        /// <summary>
        /// Creates a new <see cref="StaticStringResponseBuilder"/> object.
        /// </summary>
        /// <param name="response">The response content.</param>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="contentType">The response content type.</param>
        /// <param name="requiresAuthentication">Whether the response requires an authenticated request.</param>
        public StaticStringResponseBuilder(
            string? response,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string contentType = MediaTypeNames.Application.Xml,
            bool requiresAuthentication = true)
        {
            _response = response;
            _statusCode = statusCode;
            _contentType = contentType;
            RequiresAuthentication = requiresAuthentication;
        }

        /// <inheritdoc />
        public virtual Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = (_response ?? string.Empty).ToHttpContent(new MediaTypeWithQualityHeaderValue(_contentType))
            };

            return Task.FromResult(response);
        }
    }
}
