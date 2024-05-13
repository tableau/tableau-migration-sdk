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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Default implementation of <see cref="IHttpClient"/> that wraps and manages a concrete <see cref="HttpClient"/>.
    /// </summary>
    internal sealed class DefaultHttpClient : IHttpClient
    {
        private readonly HttpClient _innerHttpClient;
        private readonly IHttpContentSerializer _serializer;

        public DefaultHttpClient(
            HttpClient httpClient,
            IHttpContentSerializer serializer)
        {
            _innerHttpClient = httpClient;
            _serializer = serializer;

            //Timeout is controlled through a request timeout strategy instead of the HTTP client.
            _innerHttpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        #region - IHttpClient Implementation -

        #region - SendAsync -

        /// <inheritdoc />
        public async Task<IHttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => await CreateResponseAsync(_innerHttpClient.SendAsync(Guard.AgainstNull(request, nameof(request)), cancellationToken)).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IHttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
            => await CreateResponseAsync(_innerHttpClient.SendAsync(Guard.AgainstNull(request, nameof(request)), completionOption, cancellationToken)).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IHttpResponseMessage<TResponse>> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
            where TResponse : class => await CreateDeserializedResponseAsync<TResponse>(_innerHttpClient.SendAsync(Guard.AgainstNull(request, nameof(request)), cancellationToken), cancellationToken).ConfigureAwait(false);

        #endregion

        #endregion

        #region - Private Methods -

        private static async Task<IHttpResponseMessage> CreateResponseAsync(Task<HttpResponseMessage> getResponse)
        {
            var response = await getResponse.ConfigureAwait(false);

            return new DefaultHttpResponseMessage(response);
        }

        private async Task<IHttpResponseMessage<TResponse>> CreateDeserializedResponseAsync<TResponse>(Task<HttpResponseMessage> getResponse, CancellationToken cancellationToken)
            where TResponse : class
        {
            var response = await getResponse.ConfigureAwait(false);

            var deserialized = await _serializer.DeserializeAsync<TResponse>(response.Content, cancellationToken).ConfigureAwait(false);

            return new DefaultHttpResponseMessage<TResponse>(response, deserialized);
        }

        #endregion
    }
}
