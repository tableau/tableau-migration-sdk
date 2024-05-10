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
    /// Interface for an object that contains the input given for a <see cref="IApiClient"/>, 
    /// used to bootstrap api client dependency injection.
    /// </summary>
    /// <remarks>
    /// In almost all cases it is preferrable to inject the <see cref="IApiClient"/> object, 
    /// this interface is only intended to be used to build <see cref="IApiClient"/> object.
    /// </remarks>
    public interface IApiClientInput
    {
        /// <summary>
        /// Gets the site connection configuration to initialize the <see cref="IApiClient"/> with.
        /// </summary>
        TableauSiteConnectionConfiguration SiteConnectionConfiguration { get; }

        /// <summary>
        /// Gets the factory to access content reference finders with.
        /// </summary>
        IContentReferenceFinderFactory ContentReferenceFinderFactory { get; }

        /// <summary>
        /// Gets the file store to use.
        /// </summary>
        IContentFileStore FileStore { get; }
    }
}
