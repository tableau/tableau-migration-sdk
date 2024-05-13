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
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net
{
    internal class HttpStreamProcessor : StreamProcessorBase, IHttpStreamProcessor
    {
        private readonly IHttpClient _httpClient;

        public HttpStreamProcessor(
            IHttpClient httpClient,
            IConfigReader configReader)
            : base(configReader)
        {
            _httpClient = httpClient;
        }

        public virtual async Task<IEnumerable<IHttpResponseMessage<TResponse>>> ProcessAsync<TResponse>(
            Stream stream,
            Func<byte[], int, HttpRequestMessage> buildChunkRequest,
            CancellationToken cancel)
            where TResponse : class
        {
            return await ProcessAsync(
                stream,
                buildChunkRequest,
                async (request, c) =>
                {
                    var response = await _httpClient.SendAsync<TResponse>(request, c).ConfigureAwait(false);

                    return (response, response.IsSuccessStatusCode);
                },
                cancel)
                .ConfigureAwait(false);
        }
    }
}
