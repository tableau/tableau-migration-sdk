﻿// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Net.Http;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build HTTP GET requests.
    /// </summary>
    internal sealed class HttpGetRequestBuilder : HttpRequestBuilder<HttpGetRequestBuilder, IHttpGetRequestBuilder>, IHttpGetRequestBuilder
    {
        /// <inheritdoc/>
        internal override HttpMethod Method { get; } = HttpMethod.Get;

        /// <summary>
        /// Creates a new <see cref="HttpGetRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI for the request.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        public HttpGetRequestBuilder(Uri uri, IHttpClient httpClient)
            : base(uri, httpClient)
        { }
    }
}
