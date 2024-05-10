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

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for a class that can build HTTP requests.
    /// </summary>
    public interface IHttpRequestBuilder
    {
        /// <summary>
        /// Gets the built HTTP request.
        /// </summary>
        HttpRequestMessage Request { get; }

        /// <summary>
        /// Send the HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<IHttpResponseMessage> SendAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Send the HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<IHttpResponseMessage> SendAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken);

        /// <summary>
        /// Send the HTTP request as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResponse">The type of response.</typeparam>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<IHttpResponseMessage<TResponse>> SendAsync<TResponse>(CancellationToken cancellationToken)
            where TResponse : class;
    }

    /// <summary>
    /// Interface for a class that can build HTTP requests.
    /// </summary>
    public interface IHttpRequestBuilder<TBuilderInterface> : IHttpRequestBuilder
    {
        /// <summary>
        /// Adds a header to the request.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface AddHeader(string name, string value);

        /// <summary>
        /// Sets the Accept header for the request.
        /// </summary>
        /// <param name="contentType">The response content type to accept from the server.</param>
        /// <param name="clearExisting">True to clear any existing Accept content types, false otherwise.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface Accept(MediaTypeWithQualityHeaderValue contentType, bool clearExisting);

        /// <summary>
        /// Sets the Accept header to accept XML for the request.
        /// </summary>
        /// <param name="clearExisting">True to clear any existing Accept content types, false otherwise.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface AcceptXml(bool clearExisting);

        /// <summary>
        /// Sets the Accept header to accept JSON for the request.
        /// </summary>
        /// <param name="clearExisting">True to clear any existing Accept content types, false otherwise.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface AcceptJson(bool clearExisting);
    }
}