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
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api
{
    internal sealed class ScopedApiClientFactory : IScopedApiClientFactory
    {
        private readonly IServiceProvider _services;

        public ScopedApiClientFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public IApiClient Initialize(TableauSiteConnectionConfiguration scopedSiteConnection,
            IContentReferenceFinderFactory? finderFactoryOverride = null,
            IContentFileStore? fileStoreOverride = null)
        {
            var apiClientInput = _services.GetRequiredService<IApiClientInputInitializer>();
            apiClientInput.Initialize(scopedSiteConnection, finderFactoryOverride, fileStoreOverride);

            var requestFactoryInput = _services.GetRequiredService<IRequestBuilderFactoryInputInitializer>();
            requestFactoryInput.Initialize(scopedSiteConnection.ServerUrl);

            return _services.GetRequiredService<IApiClient>();
        }
    }
}
