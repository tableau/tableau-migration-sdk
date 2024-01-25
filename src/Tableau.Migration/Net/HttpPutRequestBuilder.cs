using System;
using System.Net.Http;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build HTTP PUT requests.
    /// </summary>
    internal sealed class HttpPutRequestBuilder : HttpContentRequestBuilder<HttpPutRequestBuilder, IHttpPutRequestBuilder>, IHttpPutRequestBuilder
    {
        /// <inheritdoc/>
        internal override HttpMethod Method { get; } = HttpMethod.Put;

        /// <summary>
        /// Creates a new <see cref="HttpPutRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI for the request.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        /// <param name="serializer">The serializer used to (de)serialize request content.</param>
        public HttpPutRequestBuilder(Uri uri, IHttpClient httpClient, IHttpContentSerializer serializer)
            : base(uri, httpClient, serializer)
        { }
    }
}
