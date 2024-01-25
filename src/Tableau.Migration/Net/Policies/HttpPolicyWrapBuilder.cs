using System.Collections.Generic;
using System.Net.Http;
using Polly;

namespace Tableau.Migration.Net.Policies
{
    internal class HttpPolicyWrapBuilder
        : IHttpPolicyWrapBuilder
    {
        private readonly IEnumerable<IHttpPolicyBuilder> _httpPolicyBuilders;

        public HttpPolicyWrapBuilder(
            IEnumerable<IHttpPolicyBuilder> httpPolicyBuilders)
        {
            _httpPolicyBuilders = httpPolicyBuilders;
        }

        public IAsyncPolicy<HttpResponseMessage> GetRequestPolicies(
            HttpRequestMessage httpRequest)
        {
            // TODO: Define policies for Http Request Messages
            // Default: NoOp
            // W-12406164: Network Client - Client Throttling - Rate Limit
            // Additional policies that could be defined later:
            // Circuit-breaker
            // Cache
            // Fallback
            var policies = new List<IAsyncPolicy<HttpResponseMessage>>();

            foreach (var policyBuilder in _httpPolicyBuilders)
            {
                var policy = policyBuilder.Build(httpRequest);

                if (policy is not null)
                {
                    policies.Add(policy);
                }
            }

            if (policies.Count == 1)
            {
                return policies[0];
            }

            return Policy.WrapAsync(policies.ToArray());
        }
    }
}
