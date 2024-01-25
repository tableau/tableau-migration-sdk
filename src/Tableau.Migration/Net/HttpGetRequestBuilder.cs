using System;
using System.Net.Http;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build HTTP GET requests.
    /// </summary>
    internal sealed class HttpGetRequestBuilder : HttpRequestBuilder<HttpGetRequestBuilder, IHttpGetRequestBuilder>, IHttpGetRequestBuilder
    {
        /// <inheritdoc/>
        internal override HttpMethod Method { get; } = HttpMethod.Get;

        /// <summary>
        /// Creates a new <see cref="HttpGetRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI for the request.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        public HttpGetRequestBuilder(Uri uri, IHttpClient httpClient)
            : base(uri, httpClient)
        { }
    }
}
