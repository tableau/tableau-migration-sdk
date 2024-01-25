using System.Net.Http;
using Polly;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class MaxConcurrencyPolicyBuilder
        : IHttpPolicyBuilder
    {
        private readonly IConfigReader _configReader;

        public MaxConcurrencyPolicyBuilder(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public IAsyncPolicy<HttpResponseMessage>? Build(HttpRequestMessage httpRequest)
        {
            var sdkOptions = _configReader
                .Get()
                .Network
                .Resilience;

            return sdkOptions.ConcurrentRequestsLimitEnabled
                ? Policy.BulkheadAsync<HttpResponseMessage>(
                    sdkOptions.MaxConcurrentRequests,
                    sdkOptions.ConcurrentWaitingRequestsOnQueue)
                : null;
        }
    }
}
