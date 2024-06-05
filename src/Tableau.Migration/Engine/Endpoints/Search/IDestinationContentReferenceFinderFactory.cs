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

using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Interface for an object that can create destination content reference finders
    /// based on content type.
    /// </summary>
    public interface IDestinationContentReferenceFinderFactory : IContentReferenceFinderFactory
    {
        /// <summary>
        /// Gets or creates a destination content reference finder for a given content type. 
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The content reference finder.</returns>
        IDestinationContentReferenceFinder<TContent> ForDestinationContentType<TContent>()
            where TContent : class, IContentReference;
    }
}
