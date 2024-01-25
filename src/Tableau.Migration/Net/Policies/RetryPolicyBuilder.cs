using System.Collections.Generic;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;
using Polly.RateLimit;
using Polly.Timeout;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class RetryPolicyBuilder
        : IHttpPolicyBuilder
    {
        private readonly IConfigReader _configReader;

        public RetryPolicyBuilder(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public IAsyncPolicy<HttpResponseMessage>? Build(HttpRequestMessage httpRequest)
        {
            var resilienceOptions = _configReader
                .Get()
                .Network
                .Resilience;

            var retryIntervals = resilienceOptions.RetryIntervals;

            if (!resilienceOptions.RetryEnabled
                || retryIntervals.Length == 0)
            {
                return null;
            }

            var policy = HttpPolicyExtensions.HandleTransientHttpError();

            var retryStatusCodes = resilienceOptions.RetryOverrideResponseCodes;

            if (retryStatusCodes is not null &&
                retryStatusCodes.Length > 0)
            {
                var hashStatusCodes = new HashSet<int>(retryStatusCodes);

                policy = Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult(result => hashStatusCodes.Contains((int)result.StatusCode));
            }

            return policy
                .Or<TimeoutRejectedException>()
                .Or<RateLimitRejectedException>()
                .WaitAndRetryAsync(retryIntervals);
        }
    }
}
