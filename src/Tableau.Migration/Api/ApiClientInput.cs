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
using Tableau.Migration.Api.Search;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Default <see cref="IApiClientInput"/> and <see cref="IApiClientInputInitializer"/> implementation.
    /// </summary>
    internal sealed class ApiClientInput : IApiClientInputInitializer
    {
        private readonly IServiceProvider _services;
        private readonly ISharedResourcesLocalizer _localizer;

        public ApiClientInput(IServiceProvider services, ISharedResourcesLocalizer localizer)
        {
            _services = services;
            _localizer = localizer;
        }

        private void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(_localizer[SharedResourceKeys.ApiClientInputNotInitializedError]);
            }
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public TableauSiteConnectionConfiguration SiteConnectionConfiguration
        {
            get
            {
                EnsureInitialized();
                return _siteConnectionConfiguration;
            }
            private set => _siteConnectionConfiguration = value;
        }
        private TableauSiteConnectionConfiguration _siteConnectionConfiguration;

        /// <inheritdoc />
        public IContentReferenceFinderFactory ContentReferenceFinderFactory
        {
            get
            {
                EnsureInitialized();
                return Guard.AgainstNull(_contentReferenceFinderFactory, nameof(ContentReferenceFinderFactory));
            }
            private set => _contentReferenceFinderFactory = value;
        }
        private IContentReferenceFinderFactory? _contentReferenceFinderFactory;

        /// <inheritdoc />
        public IContentFileStore FileStore
        {
            get
            {
                EnsureInitialized();
                return Guard.AgainstNull(_fileStore, nameof(FileStore));
            }
            private set => _fileStore = value;
        }
        private IContentFileStore? _fileStore;

        /// <inheritdoc />
        public void Initialize(TableauSiteConnectionConfiguration siteConnectionConfig,
            IContentReferenceFinderFactory? finderFactoryOverride = null,
            IContentFileStore? fileStoreOverride = null)
        {
            SiteConnectionConfiguration = siteConnectionConfig;
            ContentReferenceFinderFactory = finderFactoryOverride
                ?? _services.GetRequiredService<ApiContentReferenceFinderFactory>();

            FileStore = fileStoreOverride
                ?? _services.GetRequiredService<EncryptedFileStore>();

            IsInitialized = true;
        }
    }
}
