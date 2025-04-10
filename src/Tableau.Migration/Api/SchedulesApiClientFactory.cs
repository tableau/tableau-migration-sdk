//
//  Copyright (c) 2025, Salesforce, Inc.
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
using Tableau.Migration.Config;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class SchedulesApiClientFactory : ISchedulesApiClientFactory
    {
        private readonly IRestRequestBuilderFactory _restRequestBuilderFactory;
        private readonly IContentReferenceFinderFactory _finderFactory;
        private readonly IContentCacheFactory _contentCacheFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISharedResourcesLocalizer _sharedResourcesLocalizer;
        private readonly IConfigReader _configReader;

        public SchedulesApiClientFactory(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            IContentCacheFactory contentCacheFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IConfigReader configReader)
        {
            _restRequestBuilderFactory = restRequestBuilderFactory;
            _finderFactory = finderFactory;
            _contentCacheFactory = contentCacheFactory;
            _loggerFactory = loggerFactory;
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
            _configReader = configReader;
        }

        public ISchedulesApiClient Create()
            => new SchedulesApiClient(
                _restRequestBuilderFactory,
                _finderFactory,
                _contentCacheFactory,
                _loggerFactory,
                _sharedResourcesLocalizer,
                _configReader);
    }
}