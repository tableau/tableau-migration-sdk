using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class SimpleCachedClientThrottlePolicyBuilder
        : SimpleCachedHttpPolicyBuilder, IHttpPolicyBuilder
    {
        private readonly IConfigReader _configReader;

        public SimpleCachedClientThrottlePolicyBuilder(
            ClientThrottlePolicyBuilder policyBuilder,
            IConfigReader configReader)
            : base(policyBuilder)
        {
            _configReader = configReader;
        }

        protected override string GetCachedConfigurationKey()
        {
            var resilienceOptions = _configReader
                .Get()
                .Network
                .Resilience;

            return $"{resilienceOptions.ClientThrottleEnabled}_" +
                $"{resilienceOptions.MaxReadRequests}_" +
                $"{resilienceOptions.MaxReadRequestsInterval}_" +
                $"{resilienceOptions.MaxBurstReadRequests}_" +
                $"{resilienceOptions.MaxPublishRequests}_" +
                $"{resilienceOptions.MaxPublishRequestsInterval}_" +
                $"{resilienceOptions.MaxBurstPublishRequests}";
        }
    }
}
