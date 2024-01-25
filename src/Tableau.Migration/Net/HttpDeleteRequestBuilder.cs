using System;
using System.Net.Http;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build HTTP DELETE requests.
    /// </summary>
    internal sealed class HttpDeleteRequestBuilder : HttpRequestBuilder<HttpDeleteRequestBuilder, IHttpDeleteRequestBuilder>, IHttpDeleteRequestBuilder
    {
        /// <inheritdoc/>
        internal override HttpMethod Method { get; } = HttpMethod.Delete;

        /// <summary>
        /// Creates a new <see cref="HttpDeleteRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI for the request.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        public HttpDeleteRequestBuilder(Uri uri, IHttpClient httpClient)
            : base(uri, httpClient)
        { }
    }
}
