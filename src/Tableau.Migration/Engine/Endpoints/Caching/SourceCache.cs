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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Config;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Endpoints.Caching
{
    /// <summary>
    /// <see cref="ApiContentReferenceCacheBase{TContent}"/> implementation
    /// that is built from the <see cref="ISourceEndpoint"/>.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class SourceCache<TContent> : ApiContentReferenceCacheBase<TContent>, IManifestUpdateSourceContentReferenceCache<TContent>
        where TContent : class, IContentReference
    {
        private readonly IMigrationManifestEditor _manifest;
        private readonly IContentMappingRunner _mappingRunner;

        /// <inheritdoc />
        protected override string Name => $"Source Endpoint {typeof(TContent)}";

        /// <summary>
        /// Creates a new <see cref="SourceCache{TContent}"/> object.
        /// </summary>
        /// <param name="manifest">The manifest.</param>
        /// <param name="mappingRunner">The mapping runner.</param>
        /// <param name="pipeline">The migration pipeline.</param>
        /// <param name="endpoint">The source endpoint.</param>
        /// <param name="configReader">A config reader.</param>
        /// <param name="logger"><inheritdoc /></param>
        public SourceCache(IMigrationManifestEditor manifest, IContentMappingRunner mappingRunner,
            IMigrationPipeline pipeline, ISourceEndpoint endpoint, IConfigReader configReader,
            ILogger<SourceCache<TContent>> logger)
            : base(pipeline.CreateSourceCacheLoadStrategy<TContent>(), (endpoint as ISourceApiEndpoint)?.SiteApi, configReader, logger)
        {
            _manifest = manifest;
            _mappingRunner = mappingRunner;
        }

        /// <inheritdoc />
        protected override async Task ItemsLoadedAsync(IImmutableList<TContent> items, CancellationToken cancel)
        {
            await base.ItemsLoadedAsync(items, cancel).ConfigureAwait(false);

            /*
             * If new source items are found by a cache that aren't in the manifest,
             * it means we found source items that weren't found by the migration action.
             * We want to add them to the manifest so we acknowledge they were involved in the migration, 
             * even if only by reference. We run mapping for these new manifest entries 
             * so that the destination finder/caches can properly deal with reference for this source item.
             */
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();

            var itemsToAdd = items.Where(i => !manifestEntries.BySourceLocation.ContainsKey(i.Location)).ToImmutableArray();
            if(!itemsToAdd.Any())
            {
                return;
            }

            /*
             * ItemsLoadedAsync is called within the write lock of ContentReferenceCacheBase,
             * so this operation is thread safe within SourceCache<TContent>.
             * 
             * If we add additional areas that dynamically create manifest entries
             * we will need to add a lock on the manifest partition itself.
             */

            var newExpectedCount = manifestEntries.Count + itemsToAdd.Length;
            var entryBuilder = manifestEntries.GetEntryBuilder(newExpectedCount);
            var newManifestEntries = entryBuilder.CreateEntries(itemsToAdd, (i, e) => e, newExpectedCount);

            await entryBuilder.MapEntriesAsync(itemsToAdd, _mappingRunner, cancel).ConfigureAwait(false);

            foreach (var newManifestEntry in newManifestEntries)
            {
                newManifestEntry.SetSkipped(cascade: false, $"Lazy loaded after {MigrationPipelineContentType.GetDisplayNameForType(typeof(TContent))} migration.");
            }
        }

        private IMigrationManifestEntryEditor? GetManifestEntry(IContentReference? contentRef)
        {
            if (contentRef is null)
            {
                return null;
            }

            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.BySourceLocation.TryGetValue(contentRef.Location, out var manifestEntry))
            {
                return manifestEntry;
            }

            return null;
        }

        #region - IManifestUpdateSourceContentReferenceCache Implementation -

        /// <inheritdoc />
        public async Task<IMigrationManifestEntryEditor?> UpdateManifestByLocationAsync(ContentLocation location, CancellationToken cancel)
        {
            var contentRef = await ForLocationAsync(location, cancel).ConfigureAwait(false);
            return GetManifestEntry(contentRef);
        }

        /// <inheritdoc />
        public async Task<IMigrationManifestEntryEditor?> UpdateManifestByIdAsync(Guid id, CancellationToken cancel)
        {
            var contentRef = await ForIdAsync(id, cancel).ConfigureAwait(false);
            return GetManifestEntry(contentRef);
        }

        /// <inheritdoc />
        public async Task<IMigrationManifestEntryEditor?> UpdateManifestByContentUrlAsync(string contentUrl, CancellationToken cancel)
        {
            var contentRef = await ForContentUrlAsync(contentUrl, cancel).ConfigureAwait(false);
            return GetManifestEntry(contentRef);
        }

        #endregion
    }
}
