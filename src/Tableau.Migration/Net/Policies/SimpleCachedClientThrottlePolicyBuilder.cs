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
