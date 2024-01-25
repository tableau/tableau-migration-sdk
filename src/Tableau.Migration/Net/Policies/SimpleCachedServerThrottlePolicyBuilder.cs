using System.Net.Http;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal sealed class SimpleCachedServerThrottlePolicyBuilder
        : SimpleCachedHttpPolicyBuilder, IHttpPolicyBuilder
    {
        private readonly IConfigReader _configReader;

        public SimpleCachedServerThrottlePolicyBuilder(ServerThrottlePolicyBuilder policyBuilder,
            IConfigReader configReader)
            : base(policyBuilder)
        {
            _configReader = configReader;
        }

        protected override string GetRequestKey(HttpRequestMessage httpRequest)
        {
            // Single Key - Shared for every request
            return string.Empty;
        }

        protected override string GetCachedConfigurationKey()
        {
            var resilienceOptions = _configReader.Get().Network.Resilience;

            return $"{resilienceOptions.ServerThrottleEnabled}_" +
                $"{resilienceOptions.ServerThrottleLimitRetries}_" +
                $"{string.Join(";", resilienceOptions.ServerThrottleRetryIntervals)}_";
        }
    }
}
