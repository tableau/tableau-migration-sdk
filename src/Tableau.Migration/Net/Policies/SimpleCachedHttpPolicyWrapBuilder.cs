using System.Net.Http;
using Polly;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class SimpleCachedHttpPolicyWrapBuilder
        : SimpleCachedHttpPolicyContainer, IHttpPolicyWrapBuilder
    {
        private readonly HttpPolicyWrapBuilder _policyBuilder;
        private readonly IConfigReader _configReader;

        public SimpleCachedHttpPolicyWrapBuilder(
            HttpPolicyWrapBuilder policyBuilder,
            IConfigReader configReader)
        {
            _policyBuilder = policyBuilder;
            _configReader = configReader;
        }

        public IAsyncPolicy<HttpResponseMessage> GetRequestPolicies(
            HttpRequestMessage httpRequest)
        {
            return GetCachedPolicy(httpRequest)!;
        }

        protected override string GetCachedConfigurationKey()
        {
            return _configReader
                .Get()
                .Network
                .Resilience
                .ToJson();
        }

        protected override IAsyncPolicy<HttpResponseMessage>? GetFreshPolicy(
            HttpRequestMessage httpRequest)
        {
            return _policyBuilder.GetRequestPolicies(
                httpRequest);
        }
    }
}
