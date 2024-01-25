using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Default implementation of <see cref="IHttpClient"/> that wraps and manages a concrete <see cref="HttpClient"/>.
    /// </summary>
    internal sealed class DefaultHttpClient : IHttpClient
    {
        private readonly HttpClient _innerHttpClient;
        private readonly IHttpContentSerializer _serializer;

        public DefaultHttpClient(
            HttpClient httpClient,
            IHttpContentSerializer serializer)
        {
            _innerHttpClient = httpClient;
            _serializer = serializer;

            //Timeout is controlled through a request timeout policy instead of the HTTP client.
            _innerHttpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        #region - IHttpClient Implementation -

        #region - SendAsync -

        /// <inheritdoc />
        public async Task<IHttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => await CreateResponseAsync(_innerHttpClient.SendAsync(Guard.AgainstNull(request, nameof(request)), cancellationToken)).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IHttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
            => await CreateResponseAsync(_innerHttpClient.SendAsync(Guard.AgainstNull(request, nameof(request)), completionOption, cancellationToken)).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IHttpResponseMessage<TResponse>> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
            where TResponse : class => await CreateDeserializedResponseAsync<TResponse>(_innerHttpClient.SendAsync(Guard.AgainstNull(request, nameof(request)), cancellationToken), cancellationToken).ConfigureAwait(false);

        #endregion

        #endregion

        #region - Private Methods -

        private static async Task<IHttpResponseMessage> CreateResponseAsync(Task<HttpResponseMessage> getResponse)
        {
            var response = await getResponse.ConfigureAwait(false);

            return new DefaultHttpResponseMessage(response);
        }

        private async Task<IHttpResponseMessage<TResponse>> CreateDeserializedResponseAsync<TResponse>(Task<HttpResponseMessage> getResponse, CancellationToken cancellationToken)
            where TResponse : class
        {
            var response = await getResponse.ConfigureAwait(false);

            var deserialized = await _serializer.DeserializeAsync<TResponse>(response.Content, cancellationToken).ConfigureAwait(false);

            return new DefaultHttpResponseMessage<TResponse>(response, deserialized);
        }

        #endregion
    }
}
