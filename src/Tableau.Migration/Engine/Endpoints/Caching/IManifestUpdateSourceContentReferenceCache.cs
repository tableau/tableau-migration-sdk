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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Endpoints.Caching
{
    /// <summary>
    /// Interface for an object that can update the manifest dynamically
    /// by searching for source items.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IManifestUpdateSourceContentReferenceCache<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Attempts to update the manifest for a missing source location.
        /// </summary>
        /// <param name="location">The source location to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await with the manifest entry for the source information, or null if the source item could not be found.</returns>
        Task<IMigrationManifestEntryEditor?> UpdateManifestByLocationAsync(ContentLocation location, CancellationToken cancel);

        /// <summary>
        /// Attempts to update the manifest for a missing source ID.
        /// </summary>
        /// <param name="id">The source ID to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await with the manifest entry for the source information, or null if the source item could not be found.</returns>
        Task<IMigrationManifestEntryEditor?> UpdateManifestByIdAsync(Guid id, CancellationToken cancel);

        /// <summary>
        /// Attempts to update the manifest for a missing source content URL.
        /// </summary>
        /// <param name="contentUrl">The source content URL to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await with the manifest entry for the source information, or null if the source item could not be found.</returns>
        Task<IMigrationManifestEntryEditor?> UpdateManifestByContentUrlAsync(string contentUrl, CancellationToken cancel);
    }
}
