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

using System;
using System.Net.Http;
using Polly;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class ClientThrottlePolicyBuilder
        : IHttpPolicyBuilder
    {
        private readonly IConfigReader _configReader;

        public ClientThrottlePolicyBuilder(
            IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public IAsyncPolicy<HttpResponseMessage>? Build(
            HttpRequestMessage httpRequest)
        {
            var resilienceOptions = _configReader
                .Get()
                .Network
                .Resilience;

            if (!resilienceOptions.ClientThrottleEnabled)
            {
                return null;
            }

            if (httpRequest.Method == HttpMethod.Get)
            {
                return BuildReadRateLimitPolicy(
                    resilienceOptions.MaxReadRequests,
                    resilienceOptions.MaxReadRequestsInterval,
                    resilienceOptions.MaxBurstReadRequests);
            }

            return BuildPublishRateLimitPolicy(
                resilienceOptions.MaxPublishRequests,
                resilienceOptions.MaxPublishRequestsInterval,
                resilienceOptions.MaxBurstPublishRequests);
        }

        private static IAsyncPolicy<HttpResponseMessage>? BuildReadRateLimitPolicy(
            int maxReadRequests,
            TimeSpan maxReadRequestsInterval,
            int maxBurstReadRequests)
        {
            return Policy.RateLimitAsync<HttpResponseMessage>(
                maxReadRequests,
                maxReadRequestsInterval,
                maxBurstReadRequests);
        }

        private static IAsyncPolicy<HttpResponseMessage> BuildPublishRateLimitPolicy(
            int maxPublishRequests,
            TimeSpan maxPublishRequestsInterval,
            int maxBurstPublishRequests)
        {
            return Policy.RateLimitAsync<HttpResponseMessage>(
                maxPublishRequests,
                maxPublishRequestsInterval,
                maxBurstPublishRequests);
        }
    }
}
