using System;
using System.Net.Http;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build HTTP PATCH requests.
    /// </summary>
    internal sealed class HttpPatchRequestBuilder : HttpContentRequestBuilder<HttpPatchRequestBuilder, IHttpPatchRequestBuilder>, IHttpPatchRequestBuilder
    {
        /// <inheritdoc/>
        internal override HttpMethod Method { get; } = HttpMethod.Patch;

        /// <summary>
        /// Creates a new <see cref="HttpPatchRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI for the request.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        /// <param name="serializer">The serializer used to (de)serialize request content.</param>
        public HttpPatchRequestBuilder(Uri uri, IHttpClient httpClient, IHttpContentSerializer serializer)
            : base(uri, httpClient, serializer)
        { }
    }
}