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
