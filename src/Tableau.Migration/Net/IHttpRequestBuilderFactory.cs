using System;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for <see cref="IHttpRequestBuilder"/> factories.
    /// </summary>
    public interface IHttpRequestBuilderFactory
    {
        /// <summary>
        /// Creates a new <see cref="IHttpDeleteRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpDeleteRequestBuilder"/> instance.</returns>
        IHttpDeleteRequestBuilder CreateDeleteRequest(Uri uri);

        /// <summary>
        /// Creates a new <see cref="IHttpGetRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpGetRequestBuilder"/> instance.</returns>
        IHttpGetRequestBuilder CreateGetRequest(Uri uri);

        /// <summary>
        /// Creates a new <see cref="IHttpPatchRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpPatchRequestBuilder"/> instance.</returns>
        IHttpPatchRequestBuilder CreatePatchRequest(Uri uri);

        /// <summary>
        /// Creates a new <see cref="IHttpPostRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpPostRequestBuilder"/> instance.</returns>
        IHttpPostRequestBuilder CreatePostRequest(Uri uri);

        /// <summary>
        /// Creates a new <see cref="IHttpPutRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpPutRequestBuilder"/> instance.</returns>
        IHttpPutRequestBuilder CreatePutRequest(Uri uri);
    }
}