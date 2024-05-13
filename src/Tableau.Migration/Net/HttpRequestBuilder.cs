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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build HTTP requests.
    /// </summary>
    /// <typeparam name="TBuilderImpl">The builder's concrete type.</typeparam>
    /// <typeparam name="TBuilderInterface">The builder's interface type.</typeparam>
    internal abstract class HttpRequestBuilder<TBuilderImpl, TBuilderInterface> : IHttpRequestBuilder<TBuilderInterface>
        where TBuilderImpl : HttpRequestBuilder<TBuilderImpl, TBuilderInterface>, TBuilderInterface
    {
        private readonly HttpRequestMessage _request;
        private readonly Uri _uri;

        protected readonly IHttpClient HttpClient;

        /// <summary>
        /// Gets the HTTP method for created requests.
        /// </summary>
        internal abstract HttpMethod Method { get; }

        /// <inheritdoc/>
        public virtual HttpRequestMessage Request => _request;

        /// <summary>
        /// Creates a new <see cref="HttpRequestBuilder{TBuilderImpl, TBuilderInterface}"/> instance.
        /// </summary>
        /// <param name="uri">The URI for the request.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        public HttpRequestBuilder(Uri uri, IHttpClient httpClient)
        {
            _uri = uri;
            _request = new HttpRequestMessage(Method, _uri);
            HttpClient = httpClient;
        }

        /// <inheritdoc/>
        public virtual TBuilderInterface AddHeader(string name, string value)
        {
            _request.Headers.TryAddWithoutValidation(name, value);
            return (TBuilderImpl)this;
        }

        /// <inheritdoc/>
        public virtual TBuilderInterface Accept(MediaTypeWithQualityHeaderValue contentType, bool clearExisting)
        {
            if (clearExisting)
                _request.Headers.Clear();

            _request.Headers.Accept.Add(contentType);
            return (TBuilderImpl)this;
        }

        /// <inheritdoc/>
        public virtual TBuilderInterface AcceptXml(bool clearExisting) => Accept(MediaTypes.Xml, clearExisting);

        /// <inheritdoc/>
        public virtual TBuilderInterface AcceptJson(bool clearExisting) => Accept(MediaTypes.Json, clearExisting);

        /// <inheritdoc/>
        public async Task<IHttpResponseMessage> SendAsync(CancellationToken cancellationToken)
            => await HttpClient.SendAsync(Request, cancellationToken).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task<IHttpResponseMessage> SendAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken)
            => await HttpClient.SendAsync(Request, completionOption, cancellationToken).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task<IHttpResponseMessage<TResponse>> SendAsync<TResponse>(CancellationToken cancellationToken)
            where TResponse : class => await HttpClient.SendAsync<TResponse>(Request, cancellationToken).ConfigureAwait(false);
    }
}
