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
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="IContentReferenceFinderFactory"/> implementation that finds destination references
    /// from the migration manifest.
    /// </summary>
    public class ManifestDestinationContentReferenceFinderFactory 
        : IDestinationContentReferenceFinderFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Creates a new <see cref="ManifestDestinationContentReferenceFinderFactory"/> object.
        /// </summary>
        /// <param name="services">A DI service provider to create finders with.</param>
        public ManifestDestinationContentReferenceFinderFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public virtual IContentReferenceFinder<TContent> ForContentType<TContent>()
            where TContent : class, IContentReference
            => ForDestinationContentType<TContent>();

        /// <inheritdoc />
        public virtual IDestinationContentReferenceFinder<TContent> ForDestinationContentType<TContent>()
            where TContent : class, IContentReference
            => _services.GetRequiredService<IDestinationContentReferenceFinder<TContent>>();
    }
}
