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
using System.Linq;

namespace Tableau.Migration.Net
{
    internal abstract class RequestBuilderBase<TBuilder> : IRequestBuilder<TBuilder>
        where TBuilder : RequestBuilderBase<TBuilder>
    {
        private readonly IHttpRequestBuilderFactory _requestBuilderFactory;
        private readonly IQueryStringBuilder _query;
        private readonly Uri _baseUri;
        private readonly string? _path;

        public RequestBuilderBase(Uri baseUri, string path, IHttpRequestBuilderFactory requestBuilderFactory, IQueryStringBuilder query)
        {
            _baseUri = baseUri;
            _path = path;
            _requestBuilderFactory = requestBuilderFactory;
            _query = query;
        }

        public RequestBuilderBase(Uri baseUri, string path, IHttpRequestBuilderFactory requestBuilderFactory)
            : this(baseUri, path, requestBuilderFactory, new QueryStringBuilder())
        { }

        public TBuilder WithQuery(Action<IQueryStringBuilder> query)
        {
            query(_query);
            return (TBuilder)this;
        }

        public TBuilder WithQuery(string key, string value)
        {
            _query.AddOrUpdate(key, value);
            return (TBuilder)this;
        }

        internal virtual Uri BuildUri()
        {
            var uriBuilder = new UriBuilder(_baseUri);

            var segments = _path?.Split('/', StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>();

            BuildPath(segments);

            if (segments.Any())
            {
                uriBuilder.Path = String.Join("/", segments).TrimStartPath();
            }

            BuildQuery(_query);

            if (!_query.IsEmpty)
            {
                uriBuilder.Query = _query.Build();
            }

            return uriBuilder.Uri;
        }

        protected virtual void BuildPath(List<string> segments)
        { }

        protected virtual void BuildQuery(IQueryStringBuilder query) { }

        public IHttpDeleteRequestBuilder ForDeleteRequest() => _requestBuilderFactory.CreateDeleteRequest(BuildUri());

        public IHttpGetRequestBuilder ForGetRequest() => _requestBuilderFactory.CreateGetRequest(BuildUri());

        public IHttpPatchRequestBuilder ForPatchRequest() => _requestBuilderFactory.CreatePatchRequest(BuildUri());

        public IHttpPostRequestBuilder ForPostRequest() => _requestBuilderFactory.CreatePostRequest(BuildUri());

        public IHttpPutRequestBuilder ForPutRequest() => _requestBuilderFactory.CreatePutRequest(BuildUri());
    }
}
