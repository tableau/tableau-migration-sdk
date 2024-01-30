// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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
