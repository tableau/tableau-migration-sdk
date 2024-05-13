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

using Microsoft.Extensions.Logging;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Tags
{
    internal sealed class TagsApiClientFactory : ITagsApiClientFactory
    {
        private readonly IRestRequestBuilderFactory _requestBuilderFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly IHttpContentSerializer _serializer;

        public TagsApiClientFactory(IRestRequestBuilderFactory requestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer localizer,
            IHttpContentSerializer serializer)
        {
            _requestBuilderFactory = requestBuilderFactory;
            _loggerFactory = loggerFactory;
            _localizer = localizer;
            _serializer = serializer;
        }

        public ITagsApiClient Create(IContentApiClient contentApiClient)
            => new TagsApiClient(_requestBuilderFactory, _loggerFactory, _localizer, new(contentApiClient.UrlPrefix), _serializer);
    }
}
