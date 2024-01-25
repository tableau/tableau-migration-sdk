using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for an <see cref="HttpResponseMessage"/>.
    /// </summary>
    public interface IHttpResponseMessage : IDisposable
    {
        /// <summary>
        /// Gets the HTTP message version.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the content of the HTTP response message.
        /// </summary>
        HttpContent Content { get; }

        /// <summary>
        /// Gets the status code of the HTTP response.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the reason phrase which typically is sent by servers together with the status code.
        /// </summary>
        string? ReasonPhrase { get; }

        /// <summary>
        /// Gets the collection of HTTP response headers.
        /// </summary>
        HttpHeaders Headers { get; }

        /// <summary>
        /// Gets the collection of trailing headers included in an HTTP response.
        /// </summary>
        HttpHeaders TrailingHeaders { get; }

        /// <summary>
        /// Gets the request message which led to this response message.
        /// </summary>
        HttpRequestMessage? RequestMessage { get; }

        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// </summary>
        bool IsSuccessStatusCode { get; }

        /// <summary>
        /// Throws an exception if the <see cref="IsSuccessStatusCode" /> property for the HTTP response is <see langword="false" />
        /// </summary>
        /// <returns>The HTTP response message if the call is successful.</returns>
        IHttpResponseMessage EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Interface for an <see cref="HttpResponseMessage"/> and it's deserialized content.
    /// </summary>
    /// <typeparam name="TContent">The deserialized content type.</typeparam>
    public interface IHttpResponseMessage<TContent> : IHttpResponseMessage
        where TContent : class
    {
        /// <summary>
        /// Gets the deserialized content from the response.
        /// </summary>
        TContent? DeserializedContent { get; }
    }
}