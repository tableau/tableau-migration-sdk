using System;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for classes that can build HTTP requests.
    /// </summary>
    public interface IRequestBuilder
    {
        /// <summary>
        /// Creates a new <see cref="IHttpDeleteRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpDeleteRequestBuilder"/> instance.</returns>
        IHttpDeleteRequestBuilder ForDeleteRequest();

        /// <summary>
        /// Creates a new <see cref="IHttpGetRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpGetRequestBuilder"/> instance.</returns>
        IHttpGetRequestBuilder ForGetRequest();

        /// <summary>
        /// Creates a new <see cref="IHttpPatchRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpPatchRequestBuilder"/> instance.</returns>
        IHttpPatchRequestBuilder ForPatchRequest();

        /// <summary>
        /// Creates a new <see cref="IHttpPostRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpPostRequestBuilder"/> instance.</returns>
        IHttpPostRequestBuilder ForPostRequest();

        /// <summary>
        /// Creates a new <see cref="IHttpPutRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpPutRequestBuilder"/> instance.</returns>
        IHttpPutRequestBuilder ForPutRequest();
    }

    /// <summary>
    /// Interface for classes that can build URIs
    /// </summary>
    public interface IRequestBuilder<TBuilder> : IRequestBuilder
        where TBuilder : IRequestBuilder<TBuilder>
    {
        /// <summary>
        /// Configures the query for the URI.
        /// </summary>
        /// <param name="query">The callback used to build the URI's query string.</param>
        /// <returns>The current <typeparamref name="TBuilder"/> instance.</returns>
        TBuilder WithQuery(Action<IQueryStringBuilder> query);

        /// <summary>
        /// Configures the query for the URI.
        /// </summary>
        /// <param name="key">The query string key.</param>
        /// <param name="value">The query string value.</param>
        /// <returns>The current <typeparamref name="TBuilder"/> instance.</returns>
        TBuilder WithQuery(string key, string value);
    }
}