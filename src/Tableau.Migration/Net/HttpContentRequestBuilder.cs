using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build HTTP content requests.
    /// </summary>
    /// <typeparam name="TBuilderImpl">The builder's concrete type.</typeparam>
    /// <typeparam name="TBuilderInterface">The builder's interface type.</typeparam>
    internal abstract class HttpContentRequestBuilder<TBuilderImpl, TBuilderInterface> : HttpRequestBuilder<TBuilderImpl, TBuilderInterface>, IHttpContentRequestBuilder<TBuilderInterface>
        where TBuilderImpl : HttpContentRequestBuilder<TBuilderImpl, TBuilderInterface>, TBuilderInterface
    {
        /// <summary>
        /// Gets the serializer used to (de)serialize requests.
        /// </summary>
        protected readonly IHttpContentSerializer Serializer;

        /// <summary>
        /// Creates a new <see cref="HttpContentRequestBuilder{TBuilderImpl, TBuilderInterface}"/> instance.
        /// </summary>
        /// <param name="uri">The URI for the request.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        /// <param name="serializer">The serializer used to (de)serialize request content.</param>
        public HttpContentRequestBuilder(Uri uri, IHttpClient httpClient, IHttpContentSerializer serializer)
            : base(uri, httpClient)
        {
            Serializer = serializer;
        }

        /// <inheritdoc/>
        public virtual TBuilderInterface WithContent(HttpContent content)
        {
            Request.Content = content;
            return (TBuilderImpl)this;
        }

        /// <inheritdoc/>
        public virtual TBuilderInterface WithJsonContent<TContent>(TContent content)
            where TContent : class => WithContent(content, MediaTypes.Json);

        /// <inheritdoc/>
        public virtual TBuilderInterface WithXmlContent<TContent>(TContent content)
            where TContent : class => WithContent(content, MediaTypes.Xml);

        /// <inheritdoc/>
        public virtual TBuilderInterface WithContent<TContent>(TContent content, MediaTypeWithQualityHeaderValue contentType)
            where TContent : class
        {
            // Set the Accept header to match content type.
            // Usually this is what you want, i.e. if you post JSON you want JSON back,
            // but if mixed types are needed for whatever reason
            // this can be overridden by calling Accept again with another type.
            Accept(contentType, false);

            var serializedContent = Serializer.Serialize(content, contentType);

            Request.Content = serializedContent;

            return (TBuilderImpl)this;
        }

        /// <inheritdoc/>
        public virtual TBuilderInterface WithJsonContent(string content)
            => WithContent(content, MediaTypes.Json);

        /// <inheritdoc/>
        public virtual TBuilderInterface WithXmlContent(string content)
            => WithContent(content, MediaTypes.Xml);

        /// <inheritdoc/>
        public virtual TBuilderInterface WithContent(string content, MediaTypeWithQualityHeaderValue contentType)
        {
            // Set the Accept header to match content type.
            // Usually this is what you want, i.e. if you post JSON you want JSON back,
            // but if mixed types are needed for whatever reason
            // this can be overridden by calling Accept again with another type.
            Accept(contentType, false);

            Request.Content = new StringContent(content, Encoding.UTF8, contentType.MediaType!);

            return (TBuilderImpl)this;
        }
    }
}
