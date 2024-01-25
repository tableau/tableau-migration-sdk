using System.Net.Http;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class SimpleCachedRetryPolicyBuilder
        : SimpleCachedHttpPolicyBuilder, IHttpPolicyBuilder
    {
        private readonly IConfigReader _configReader;

        public SimpleCachedRetryPolicyBuilder(
            RetryPolicyBuilder policyBuilder,
            IConfigReader configReader)
            : base(policyBuilder)
        {
            _configReader = configReader;
        }

        protected override string GetRequestKey(
            HttpRequestMessage httpRequest)
        {
            // Single Key - Shared for every request
            return string.Empty;
        }

        protected override string GetCachedConfigurationKey()
        {
            var resilienceOptions = _configReader
                .Get()
                .Network
                .Resilience;

            return $"{resilienceOptions.RetryEnabled}_" +
                $"{string.Join(";", resilienceOptions.RetryIntervals)}_" +
                $"{string.Join(";", resilienceOptions.RetryOverrideResponseCodes)}";
        }
    }
}
