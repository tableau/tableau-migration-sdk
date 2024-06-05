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
    /// <see cref="IContentReferenceFinder{TContent}"/> implementation that finds source references
    /// from the migration manifest.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ManifestSourceContentReferenceFinder<TContent> 
        : ISourceContentReferenceFinder<TContent>
        where TContent : class, IContentReference
    {
        private readonly IMigrationManifestEditor _manifest;
        private readonly IContentReferenceCache _sourceCache;

        /// <summary>
        /// Creates a new <see cref="ManifestSourceContentReferenceFinder{TContent}"/> object.
        /// </summary>
        /// <param name="manifest">The manifest.</param>
        /// <param name="pipeline">The pipeline to get a source cache from.</param>
        public ManifestSourceContentReferenceFinder(IMigrationManifestEditor manifest, IMigrationPipeline pipeline)
        {
            _manifest = manifest;
            _sourceCache = pipeline.CreateSourceCache<TContent>();
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindBySourceLocationAsync(ContentLocation sourceLocation, CancellationToken cancel)
        {
            //Get the SOURCE reference for the SOURCE location.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.BySourceLocation.TryGetValue(sourceLocation, out var entry))
            {
                return entry.Source;
            }

            return await _sourceCache.ForLocationAsync(sourceLocation, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindByIdAsync(Guid id, CancellationToken cancel)
        {
            //Get the SOURCE reference for the SOURCE ID.
            var partition = _manifest.Entries.GetOrCreatePartition<TContent>();

            if (partition.BySourceId.TryGetValue(id, out var entry))
            {
                return entry.Source;
            }

            return await _sourceCache.ForIdAsync(id, cancel).ConfigureAwait(false);
        }
    }
}
