//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Interface for an object that can provide data for a <see cref="IContentReferenceCache"/>.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IContentReferenceStore<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Loads all items.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content references to cache.</returns>
        ValueTask<IImmutableList<TContent>> LoadAllAsync(CancellationToken cancel);

        /// <summary>
        /// Loads content at the given location, possibly returning more locations to opportunistically cache.
        /// </summary>
        /// <param name="searchLocation">The primary location to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content references to cache, or an unsupported result.</returns>
        ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(ContentLocation searchLocation, CancellationToken cancel);

        /// <summary>
        /// Loads content at the given ID, possibly returning more references to opportunistically cache.
        /// </summary>
        /// <param name="searchId">The primary ID to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content references to cache, or an unsupported result.</returns>
        ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(Guid searchId, CancellationToken cancel);

        /// <summary>
        /// Loads content at the given content URL, possibly returning more references to opportunistically cache.
        /// </summary>
        /// <param name="searchContentUrl">The primary content URL to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content references to cache, or an unsupported result.</returns>
        ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(string searchContentUrl, CancellationToken cancel);
    }
}
