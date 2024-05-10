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

namespace Tableau.Migration.Net
{
    internal class HttpRequestBuilderFactory : IHttpRequestBuilderFactory
    {
        private readonly IHttpClient _httpClient;
        private readonly IHttpContentSerializer _serializer;

        public HttpRequestBuilderFactory(IHttpClient httpClient, IHttpContentSerializer serializer)
        {
            _httpClient = httpClient;
            _serializer = serializer;
        }

        public IHttpDeleteRequestBuilder CreateDeleteRequest(Uri uri) => new HttpDeleteRequestBuilder(uri, _httpClient);

        public IHttpGetRequestBuilder CreateGetRequest(Uri uri) => new HttpGetRequestBuilder(uri, _httpClient);

        public IHttpPatchRequestBuilder CreatePatchRequest(Uri uri) => new HttpPatchRequestBuilder(uri, _httpClient, _serializer);

        public IHttpPostRequestBuilder CreatePostRequest(Uri uri) => new HttpPostRequestBuilder(uri, _httpClient, _serializer);

        public IHttpPutRequestBuilder CreatePutRequest(Uri uri) => new HttpPutRequestBuilder(uri, _httpClient, _serializer);
    }
}
