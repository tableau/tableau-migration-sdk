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
    /// Interface for an object that can initialize the current scope and create scoped <see cref="IApiClient"/> objects.
    /// </summary>
    public interface IScopedApiClientFactory
    {
        /// <summary>
        /// Initializes the scoped API client configuration and creates an API client.
        /// </summary>
        /// <param name="scopedSiteConnection">The site connection configuration to use in the current DI scope.</param>
        /// <param name="finderFactoryOverride">A content reference finder factory to use in place of the default.</param>
        /// <param name="fileStoreOverride">A file store to use in place of the default.</param>
        /// <returns>The created API client.</returns>
        IApiClient Initialize(TableauSiteConnectionConfiguration scopedSiteConnection,
            IContentReferenceFinderFactory? finderFactoryOverride = null,
            IContentFileStore? fileStoreOverride = null);
    }
}
