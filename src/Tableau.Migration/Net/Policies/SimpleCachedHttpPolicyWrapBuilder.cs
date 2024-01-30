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
