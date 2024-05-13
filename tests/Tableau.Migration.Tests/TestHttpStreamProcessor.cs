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
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests
{
    public class TestHttpStreamProcessor : IHttpStreamProcessor
    {
        private readonly HttpStreamProcessor _innerProcessor;

        private readonly ImmutableList<HttpRequestMessage>.Builder _createdRequests = ImmutableList.CreateBuilder<HttpRequestMessage>();

        public IImmutableList<HttpRequestMessage> CreatedRequests => _createdRequests.ToImmutable();

        public TestHttpStreamProcessor(
            IHttpClient httpClient,
            IConfigReader configReader)
        {
            _innerProcessor = new(httpClient, configReader);
        }

        private HttpRequestMessage OnRequestCreated(HttpRequestMessage request)
        {
            _createdRequests.Add(request);
            return request;
        }

        public async Task<IEnumerable<IHttpResponseMessage<TResponse>>> ProcessAsync<TResponse>(
            Stream stream,
            Func<byte[], int, HttpRequestMessage> buildChunkRequest,
            CancellationToken cancel)
            where TResponse : class
        {
            return await _innerProcessor.ProcessAsync<TResponse>(
                stream,
                (chunk, bytesRead) =>
                {
                    var request = buildChunkRequest(chunk, bytesRead);
                    OnRequestCreated(request);
                    return request;
                },
                cancel);
        }

        public HttpRequestMessage AssertSingleRequest()
            => Assert.Single(_createdRequests);

        public void AssertNoRequests()
            => Assert.Empty(_createdRequests);
    }
}
