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

using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an object that can initialize a <see cref="IApiClientInput"/> object.
    /// </summary>
    /// <remarks>
    /// This interface is internal because it is only used to build a <see cref="IApiClientInput"/> object, 
    /// which in turn is only used to build a <see cref="IApiClientInput"/> object.
    /// End users are intended to inject the final <see cref="IApiClientInput"/> result and not bootstrap objects.
    /// </remarks>
    internal interface IApiClientInputInitializer : IApiClientInput
    {
        /// <summary>
        /// Gets whether or not the API client input has been initialized yet.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the <see cref="IApiClientInput"/> object.
        /// </summary>
        /// <param name="siteConnectionConfig">The site connection configuration to initialize the <see cref="IApiClient"/> with.</param>
        /// <param name="finderFactoryOverride">A content reference finder factory to use in place of the default.</param>
        /// <param name="fileStoreOverride">The file store to use in place of the default.</param>
        void Initialize(TableauSiteConnectionConfiguration siteConnectionConfig,
            IContentReferenceFinderFactory? finderFactoryOverride = null,
            IContentFileStore? fileStoreOverride = null);
    }
}
