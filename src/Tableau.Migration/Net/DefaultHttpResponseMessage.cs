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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Default <see cref="IHttpResponseMessage{TContent}"/> implementation.
    /// </summary>
    internal class DefaultHttpResponseMessage : IHttpResponseMessage
    {
        private readonly HttpResponseMessage _response;

        public Version Version { get; }
        public HttpContent Content { get; }
        public HttpStatusCode StatusCode { get; }
        public string? ReasonPhrase { get; }
        public HttpHeaders Headers { get; }
        public HttpHeaders TrailingHeaders { get; }
        public HttpRequestMessage? RequestMessage { get; }
        public bool IsSuccessStatusCode { get; }

        public DefaultHttpResponseMessage(HttpResponseMessage response)
        {
            _response = response;

            Version = response.Version;
            Content = response.Content;
            StatusCode = response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
            Headers = response.Headers;
            TrailingHeaders = response.TrailingHeaders;
            RequestMessage = response.RequestMessage;
            IsSuccessStatusCode = response.IsSuccessStatusCode;
        }

        public IHttpResponseMessage EnsureSuccessStatusCode()
        {
            _response.EnsureSuccessStatusCode();
            return this;
        }

        #region - IDisposable -

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _response.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Default <see cref="IHttpResponseMessage{TContent}"/> implementation.
    /// </summary>
    /// <typeparam name="TContent">The deserialized content type.</typeparam>
    internal class DefaultHttpResponseMessage<TContent> : DefaultHttpResponseMessage, IHttpResponseMessage<TContent>
        where TContent : class
    {
        public TContent? DeserializedContent { get; }

        public DefaultHttpResponseMessage(HttpResponseMessage response, TContent? deserializedContent)
            : base(response)
        {
            DeserializedContent = deserializedContent;
        }
    }
}
