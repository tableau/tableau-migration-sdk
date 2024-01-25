using System;
using System.Net.Http;
using Polly;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class ClientThrottlePolicyBuilder
        : IHttpPolicyBuilder
    {
        private readonly IConfigReader _configReader;

        public ClientThrottlePolicyBuilder(
            IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public IAsyncPolicy<HttpResponseMessage>? Build(
            HttpRequestMessage httpRequest)
        {
            var resilienceOptions = _configReader
                .Get()
                .Network
                .Resilience;

            if (!resilienceOptions.ClientThrottleEnabled)
            {
                return null;
            }

            if (httpRequest.Method == HttpMethod.Get)
            {
                return BuildReadRateLimitPolicy(
                    resilienceOptions.MaxReadRequests,
                    resilienceOptions.MaxReadRequestsInterval,
                    resilienceOptions.MaxBurstReadRequests);
            }

            return BuildPublishRateLimitPolicy(
                resilienceOptions.MaxPublishRequests,
                resilienceOptions.MaxPublishRequestsInterval,
                resilienceOptions.MaxBurstPublishRequests);
        }

        private static IAsyncPolicy<HttpResponseMessage>? BuildReadRateLimitPolicy(
            int maxReadRequests,
            TimeSpan maxReadRequestsInterval,
            int maxBurstReadRequests)
        {
            return Policy.RateLimitAsync<HttpResponseMessage>(
                maxReadRequests,
                maxReadRequestsInterval,
                maxBurstReadRequests);
        }

        private static IAsyncPolicy<HttpResponseMessage> BuildPublishRateLimitPolicy(
            int maxPublishRequests,
            TimeSpan maxPublishRequestsInterval,
            int maxBurstPublishRequests)
        {
            return Policy.RateLimitAsync<HttpResponseMessage>(
                maxPublishRequests,
                maxPublishRequestsInterval,
                maxBurstPublishRequests);
        }
    }
}
