using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests
{
    public class TestHttpStreamProcessor : IHttpStreamProcessor
    {
        private readonly HttpStreamProcessor _innerProcessor;

        private readonly ImmutableList<HttpRequestMessage>.Builder _createdRequests = ImmutableList.CreateBuilder<HttpRequestMessage>();

        public IImmutableList<HttpRequestMessage> CreatedRequests => _createdRequests.ToImmutable();

        public TestHttpStreamProcessor(
            IHttpClient httpClient,
            IConfigReader configReader)
        {
            _innerProcessor = new(httpClient, configReader);
        }

        private HttpRequestMessage OnRequestCreated(HttpRequestMessage request)
        {
            _createdRequests.Add(request);
            return request;
        }

        public async Task<IEnumerable<IHttpResponseMessage<TResponse>>> ProcessAsync<TResponse>(
            Stream stream,
            Func<byte[], int, HttpRequestMessage> buildChunkRequest,
            CancellationToken cancel)
            where TResponse : class
        {
            return await _innerProcessor.ProcessAsync<TResponse>(
                stream,
                (chunk, bytesRead) =>
                {
                    var request = buildChunkRequest(chunk, bytesRead);
                    OnRequestCreated(request);
                    return request;
                },
                cancel);
        }

        public HttpRequestMessage AssertSingleRequest()
            => Assert.Single(_createdRequests);

        public void AssertNoRequests()
            => Assert.Empty(_createdRequests);
    }
}
