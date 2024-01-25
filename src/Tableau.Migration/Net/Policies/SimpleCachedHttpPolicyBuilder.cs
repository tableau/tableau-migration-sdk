using System.Net.Http;
using Polly;

namespace Tableau.Migration.Net.Policies
{
    internal abstract class SimpleCachedHttpPolicyBuilder
        : SimpleCachedHttpPolicyContainer, IHttpPolicyBuilder
    {
        protected readonly IHttpPolicyBuilder _policyBuilder;

        public SimpleCachedHttpPolicyBuilder(
            IHttpPolicyBuilder policyBuilder)
        {
            _policyBuilder = policyBuilder;
        }

        public IAsyncPolicy<HttpResponseMessage>? Build(
            HttpRequestMessage httpRequest)
        {
            return GetCachedPolicy(httpRequest);
        }

        protected override IAsyncPolicy<HttpResponseMessage>? GetFreshPolicy(
            HttpRequestMessage httpRequest)
        {
            return _policyBuilder.Build(httpRequest);
        }
    }
}
