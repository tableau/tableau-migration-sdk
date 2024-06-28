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

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for classes that process <see cref="Stream"/>s for HTTP requests and responses.
    /// </summary>
    public interface IHttpStreamProcessor
    {
        /// <summary>
        /// Processes the stream in chunks and sends the created requests.
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="stream">The stream to process.</param>
        /// <param name="buildChunkRequest">
        /// Function to build an HTTP request from a chunk of data from the stream.
        /// The first parameter is the chunk of data, or possibly a partial chunk of data.
        /// The second parameter is the count of bytes of the chunk of data to send.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A collection of responses from the chunked requests.</returns>
        Task<IEnumerable<IHttpResponseMessage<TResponse>>> ProcessAsync<TResponse>(
            Stream stream,
            Func<byte[], int, HttpRequestMessage> buildChunkRequest,
            CancellationToken cancel) where TResponse : class;
    }
}