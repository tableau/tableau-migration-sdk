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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface wrapper of <see cref="HttpClient"/> for testability.
    /// https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines#general-idisposable-guidelines
    /// </summary>
    public interface IHttpClient
    {
        #region - SendAsync -

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request is null.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        Task<IHttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request is null.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        Task<IHttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request is null.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        Task<IHttpResponseMessage<TResponse>> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
            where TResponse : class;

        #endregion
    }
}
