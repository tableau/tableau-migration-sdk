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
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class ViewsApiClientFactory : IViewsApiClientFactory
    {
        private readonly IRestRequestBuilderFactory _restRequestBuilderFactory;
        private readonly IPermissionsApiClientFactory _permissionsClientFactory;
        private readonly IContentReferenceFinderFactory _finderFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISharedResourcesLocalizer _sharedResourcesLocalizer;
        private readonly ITagsApiClientFactory _tagsClientFactory;

        public ViewsApiClientFactory(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IPermissionsApiClientFactory permissionsClientFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            ITagsApiClientFactory tagsClientFactory)
        {
            _restRequestBuilderFactory = restRequestBuilderFactory;
            _permissionsClientFactory = permissionsClientFactory;
            _finderFactory = finderFactory;
            _loggerFactory = loggerFactory;
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
            _tagsClientFactory = tagsClientFactory;
        }

        public IViewsApiClient Create()
            => new ViewsApiClient(
                _restRequestBuilderFactory,
                _permissionsClientFactory,
                _finderFactory,
                _loggerFactory,
                _sharedResourcesLocalizer,
                _tagsClientFactory);
    }
}