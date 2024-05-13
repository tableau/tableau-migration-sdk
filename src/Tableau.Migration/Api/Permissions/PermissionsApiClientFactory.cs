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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Permissions
{
    internal sealed class PermissionsApiClientFactory : IPermissionsApiClientFactory
    {
        private readonly IRestRequestBuilderFactory _restRequestBuilderFactory;
        private readonly IHttpContentSerializer _serializer;
        private readonly ISharedResourcesLocalizer _sharedResourcesLocalizer;
        private readonly IConfigReader _configReader;
        private readonly ILoggerFactory _loggerFactory;

        public PermissionsApiClientFactory(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IHttpContentSerializer serializer,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IConfigReader configReader,
            ILoggerFactory loggerFactory)
        {
            _restRequestBuilderFactory = restRequestBuilderFactory;
            _serializer = serializer;
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
            _configReader = configReader;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public IPermissionsApiClient Create(IContentApiClient contentApiClient)
            => Create(new PermissionsUriBuilder(contentApiClient.UrlPrefix));

        /// <inheritdoc />
        public IPermissionsApiClient Create(IPermissionsUriBuilder uriBuilder)
            => new PermissionsApiClient(_restRequestBuilderFactory, _serializer, uriBuilder, _sharedResourcesLocalizer);

        /// <inheritdoc />
        public IDefaultPermissionsApiClient CreateDefaultPermissionsClient()
            => new DefaultPermissionsApiClient(this, _configReader.Get().DefaultPermissionsContentTypes, _loggerFactory, _sharedResourcesLocalizer);
    }
}
