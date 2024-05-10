//
//  Copyright (c) 2024, Salesforce, Inc.
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Tests.Unit.Net.Handlers
{
    public class MockDelegatingHandler : DelegatingHandler
    {
        private readonly List<HttpRequestMessage> _sentRequests = new();
        private readonly Func<HttpRequestMessage, HttpResponseMessage>? _onRequest;

        public IImmutableList<HttpRequestMessage> SentRequests => _sentRequests.ToImmutableArray();

        public MockDelegatingHandler(Func<HttpRequestMessage, HttpResponseMessage>? onRequest = null)
        {
            _onRequest = onRequest;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = _onRequest?.Invoke(request);

            return Task.FromResult(response ?? new HttpResponseMessage());
        }
    }
}
