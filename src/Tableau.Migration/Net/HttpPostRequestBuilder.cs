using System;
using System.Net.Http;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build HTTP POST requests.
    /// </summary>
    internal sealed class HttpPostRequestBuilder : HttpContentRequestBuilder<HttpPostRequestBuilder, IHttpPostRequestBuilder>, IHttpPostRequestBuilder
    {
        /// <inheritdoc/>
        internal override HttpMethod Method { get; } = HttpMethod.Post;

        /// <summary>
        /// Creates a new <see cref="HttpPostRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI for the request.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        /// <param name="serializer">The serializer used to (de)serialize request content.</param>
        public HttpPostRequestBuilder(Uri uri, IHttpClient httpClient, IHttpContentSerializer serializer)
            : base(uri, httpClient, serializer)
        { }
    }
}
