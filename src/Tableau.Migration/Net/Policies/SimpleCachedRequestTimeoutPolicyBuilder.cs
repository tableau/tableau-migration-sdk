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
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class SimpleCachedRequestTimeoutPolicyBuilder
        : SimpleCachedHttpPolicyBuilder, IHttpPolicyBuilder
    {
        private const string RequestKey = "request";
        private const string FileTransferRequestKey = "fileTransferRequest";

        private readonly IConfigReader _configReader;

        public SimpleCachedRequestTimeoutPolicyBuilder(
            RequestTimeoutPolicyBuilder policyBuilder,
            IConfigReader configReader)
            : base(policyBuilder)
        {
            _configReader = configReader;
        }

        protected override string GetRequestKey(
            HttpRequestMessage httpRequest)
        {
            // Double Key - Shared for every request/file-transfer request
            return RequestTimeoutPolicyBuilder.IsFileTransferRequest(httpRequest) ? RequestKey : FileTransferRequestKey;
        }

        protected override string GetCachedConfigurationKey()
        {
            var resilienceOptions = _configReader
                .Get()
                .Network
                .Resilience;

            return $"{resilienceOptions.PerRequestTimeout}_{resilienceOptions.PerFileTransferRequestTimeout}";
        }
    }
}
