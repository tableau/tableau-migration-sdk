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

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Default <see cref="IMigrationEndpointFactory"/> implementation.
    /// </summary>
    public class MigrationEndpointFactory : IMigrationEndpointFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDestinationContentReferenceFinderFactory _destinationFinderFactory;
        private readonly ISourceContentReferenceFinderFactory _sourceFinderFactory;
        private readonly IContentFileStore _fileStore;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="MigrationEndpointFactory"/> object.
        /// </summary>
        /// <param name="serviceScopeFactory">A service scope factory to define an API client scope with.</param>
        /// <param name="sourceFinderFactory">A source content reference finder factory.</param>
        /// <param name="destinationFinderFactory">A destination content reference finder factory.</param>
        /// <param name="fileStore">The file store to use.</param>
        /// <param name="loggerFactory">The logger factory to use.</param>
        /// <param name="localizer">A string localizer.</param>
        public MigrationEndpointFactory(IServiceScopeFactory serviceScopeFactory,
            ISourceContentReferenceFinderFactory sourceFinderFactory,
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            IContentFileStore fileStore,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer localizer)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _destinationFinderFactory = destinationFinderFactory;
            _sourceFinderFactory = sourceFinderFactory;
            _fileStore = fileStore;
            _loggerFactory = loggerFactory;
            _localizer = localizer;
        }

        /// <inheritdoc />
        public IDestinationEndpoint CreateDestination(IMigrationPlan plan)
        {
            if (plan.Destination is ITableauApiEndpointConfiguration apiConfig)
            {
                return new TableauApiDestinationEndpoint(_serviceScopeFactory, apiConfig, _destinationFinderFactory, _fileStore, _loggerFactory, _localizer);
            }

            throw new ArgumentException($"Cannot create a destination endpoint for type {plan.Source.GetType()}");
        }

        /// <inheritdoc />
        public ISourceEndpoint CreateSource(IMigrationPlan plan)
        {
            if (plan.Source is ITableauApiEndpointConfiguration apiConfig)
            {
                return new TableauApiSourceEndpoint(_serviceScopeFactory, apiConfig, _sourceFinderFactory, _fileStore, _loggerFactory, _localizer);
            }

            throw new ArgumentException($"Cannot create a source endpoint for type {plan.Source.GetType()}");
        }
    }
}
