﻿//
//  Copyright (c) 2025, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Handlers
{
    /// <summary>
    /// Handler that will add the SDK user agent to all requests
    /// </summary>
    internal class UserAgentHeaderHttpHandler : DelegatingHandler
    {
        private readonly IUserAgentProvider _userAgentProvider;

        public UserAgentHeaderHttpHandler(IUserAgentProvider userAgentProvider)
        {
            _userAgentProvider = userAgentProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.Clear();
            request.Headers.UserAgent.TryParseAdd(_userAgentProvider.UserAgent);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
