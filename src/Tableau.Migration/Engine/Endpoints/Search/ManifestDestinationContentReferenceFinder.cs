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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="IDestinationContentReferenceFinder{TContent}"/> implementation 
    /// that uses the mapped manifest information to find destination content, 
    /// falling back to API loading.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ManifestDestinationContentReferenceFinder<TContent>
        : IDestinationContentReferenceFinder<TContent>
        where TContent : class, IContentReference
    {
        private readonly IMigrationManifestEditor _manifest;
        private readonly IContentReferenceCache _destinationCache;

        /// <summary>
        /// Creates a new <see cref="ManifestDestinationContentReferenceFinder{TContent}"/> object.
        /// </summary>
        /// <param name="manifest">The migration manifest.</param>
        /// <param name="pipeline">The pipeline to get a destination cache from.</param>
        public ManifestDestinationContentReferenceFinder(IMigrationManifestEditor manifest, IMigrationPipeline pipeline)
        {
            _manifest = manifest;
            _destinationCache = pipeline.CreateDestinationCache<TContent>();
        }

        #region - IDestinationContentReferenceFinder Implementation -

        /// <inheritdoc />
        public async Task<IContentReference?> FindBySourceLocationAsync(ContentLocation sourceLocation, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the SOURCE location.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.BySourceLocation.TryGetValue(sourceLocation, out var entry))
            {
                if(entry.Destination is not null)
                {
                    return entry.Destination;
                }

                return await _destinationCache.ForLocationAsync(entry.MappedLocation, cancel).ConfigureAwait(false);
            }
             
            return null;
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindByMappedLocationAsync(ContentLocation mappedLocation, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the DESTINATION location.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.ByMappedLocation.TryGetValue(mappedLocation, out var entry) && entry.Destination is not null)
            {
                return entry.Destination;
            }

            return await _destinationCache.ForLocationAsync(mappedLocation, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindBySourceIdAsync(Guid sourceId, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the SOURCE ID.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.BySourceId.TryGetValue(sourceId, out var entry))
            {
                return await FindBySourceLocationAsync(entry.Source.Location, cancel).ConfigureAwait(false);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindBySourceContentUrlAsync(string contentUrl, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the SOURCE content URL.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.BySourceContentUrl.TryGetValue(contentUrl, out var entry))
            {
                return await FindBySourceLocationAsync(entry.Source.Location, cancel).ConfigureAwait(false);
            }

            return null;
        }

        #endregion

        #region - IContentReferenceFinder Implementation -

        /// <inheritdoc />
        public async Task<IContentReference?> FindByIdAsync(Guid id, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the DESTINATION ID.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.ByDestinationId.TryGetValue(id, out var entry) && entry.Destination is not null)
            {
                return entry.Destination;
            }

            return await _destinationCache.ForIdAsync(id, cancel).ConfigureAwait(false);
        }

        #endregion
    }
}
