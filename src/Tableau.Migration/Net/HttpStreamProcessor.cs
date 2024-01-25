using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net
{
    internal class HttpStreamProcessor : StreamProcessorBase, IHttpStreamProcessor
    {
        private readonly IHttpClient _httpClient;

        public HttpStreamProcessor(
            IHttpClient httpClient,
            IConfigReader configReader)
            : base(configReader)
        {
            _httpClient = httpClient;
        }

        public virtual async Task<IEnumerable<IHttpResponseMessage<TResponse>>> ProcessAsync<TResponse>(
            Stream stream,
            Func<byte[], int, HttpRequestMessage> buildChunkRequest,
            CancellationToken cancel)
            where TResponse : class
        {
            return await ProcessAsync(
                stream,
                buildChunkRequest,
                async (request, c) =>
                {
                    var response = await _httpClient.SendAsync<TResponse>(request, c).ConfigureAwait(false);

                    return (response, response.IsSuccessStatusCode);
                },
                cancel)
                .ConfigureAwait(false);
        }
    }
}
