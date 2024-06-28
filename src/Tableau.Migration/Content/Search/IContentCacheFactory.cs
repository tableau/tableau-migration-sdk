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

using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Interface for an object that can create <see cref="IContentCache{TContent}"/> caches for a given content type.
    /// </summary>
    /// <remarks>Implementations should be thread safe due to parallel migration processing.</remarks>
    public interface IContentCacheFactory
    {
        /// <summary>
        /// Gets or creates a content cache for a given content type. 
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="throwIfNotAvailable">True to throw if the cache is not available/registered, false otherwise.</param>
        /// <returns>The content cache.</returns>
        IContentCache<TContent>? ForContentType<TContent>([DoesNotReturnIf(true)] bool throwIfNotAvailable)
            where TContent : class, IContentReference;
    }
}
