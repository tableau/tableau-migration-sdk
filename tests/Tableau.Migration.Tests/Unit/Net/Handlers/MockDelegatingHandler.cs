using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Tests.Unit.Net.Handlers
{
    public class MockDelegatingHandler : DelegatingHandler
    {
        private readonly List<HttpRequestMessage> _sentRequests = new();
        private readonly Func<HttpRequestMessage, HttpResponseMessage>? _onRequest;

        public IImmutableList<HttpRequestMessage> SentRequests => _sentRequests.ToImmutableArray();

        public MockDelegatingHandler(Func<HttpRequestMessage, HttpResponseMessage>? onRequest = null)
        {
            _onRequest = onRequest;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = _onRequest?.Invoke(request);

            return Task.FromResult(response ?? new HttpResponseMessage());
        }
    }
}
