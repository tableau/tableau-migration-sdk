using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using Polly;

namespace Tableau.Migration.Net.Policies
{
    internal abstract class SimpleCachedHttpPolicyContainer
    {
        private string _cachedConfigurationKey = string.Empty;
        private readonly ConcurrentDictionary<string, IAsyncPolicy<HttpResponseMessage>?> _requestPolicies = new();
        private SpinLock _lock = new();

        public IAsyncPolicy<HttpResponseMessage>? GetCachedPolicy(
            HttpRequestMessage httpRequest)
        {
            RefreshCachedConfiguration();

            return GetPolicy(httpRequest);
        }

        private void RefreshCachedConfiguration()
        {
            var configurationKey = GetCachedConfigurationKey();
            var lockTaken = false;

            while (!string.Equals(configurationKey, _cachedConfigurationKey))
            {
                try
                {
                    _lock.TryEnter(ref lockTaken);

                    if (lockTaken)
                    {
                        _requestPolicies.Clear();

                        _cachedConfigurationKey = configurationKey;
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _lock.Exit();
                    }
                }
            };
        }

        private IAsyncPolicy<HttpResponseMessage>? GetPolicy(
            HttpRequestMessage httpRequest)
        {
            var requestKey = GetRequestKey(httpRequest);
            IAsyncPolicy<HttpResponseMessage>? policy;

            while (!_requestPolicies.TryGetValue(
                requestKey,
                out policy))
            {
                policy = GetFreshPolicy(httpRequest);

                if (_requestPolicies.TryAdd(
                    requestKey,
                    policy))
                {
                    return policy;
                }
            };

            return policy;
        }

        protected virtual string GetRequestKey(
            HttpRequestMessage httpRequest)
        {
            return httpRequest.GetPolicyRequestKey();
        }

        protected abstract string GetCachedConfigurationKey();

        protected abstract IAsyncPolicy<HttpResponseMessage>? GetFreshPolicy(
            HttpRequestMessage httpRequest);
    }
}
