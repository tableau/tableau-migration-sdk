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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Endpoints.Caching
{
    /// <summary>
    /// <see cref="ApiContentReferenceCacheBase{TContent}"/> implementation
    /// that falls back to bulk API listing when destination information is not found in the manifest.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class DestinationCache<TContent> : ApiContentReferenceCacheBase<TContent>
        where TContent : class, IContentReference
    {
        private readonly IMigrationManifestContentTypePartitionEditor _manifestEntries;

        /// <inheritdoc />
        protected override string Name => $"Destination Endpoint {typeof(TContent)}";

        /// <summary>
        /// Creates a new <see cref="DestinationCache{TContent}"/>
        /// </summary>
        /// <param name="pipeline">The migration pipeline.</param>
        /// <param name="endpoint">The destination endpoint.</param>
        /// <param name="configReader">A config reader.</param>
        /// <param name="manifest">A migration manifest.</param>
        /// <param name="logger"><inheritdoc /></param>
        public DestinationCache(
            IMigrationPipeline pipeline,
            IDestinationEndpoint endpoint,
            IConfigReader configReader,
            IMigrationManifestEditor manifest,
            ILogger<DestinationCache<TContent>> logger)
            : base(pipeline.CreateDestinationCacheLoadStrategy<TContent>(), (endpoint as IDestinationApiEndpoint)?.SiteApi, configReader, logger)
        {
            _manifestEntries = manifest.Entries.GetOrCreatePartition<TContent>();
        }

        /// <inheritdoc />
        protected override async Task ItemsLoadedAsync(IImmutableList<TContent> items, CancellationToken cancel)
        {
            foreach(var item in items)
            {
                //Assign this info to the manifest if there's an entry with our mapped location.
                //This updates any ID/other information that may have changed since last run.
                if (_manifestEntries.ByMappedLocation.TryGetValue(item.Location, out var manifestEntry))
                {
                    manifestEntry.DestinationFound(new ContentReferenceStub(item));
                }
            }

            await base.ItemsLoadedAsync(items, cancel);
        }

        /// <inheritdoc />
        public override async Task<IContentReference?> ForIdAsync(Guid id, CancellationToken cancel)
        {
            if (_manifestEntries.ByDestinationId.TryGetValue(id, out var entry))
            {
                if (entry.Destination is not null)
                {
                    return new ContentReferenceStub(entry.Destination);
                }
            }

            return await base.ForIdAsync(id, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<IContentReference?> ForLocationAsync(ContentLocation location, CancellationToken cancel)
        {
            if (_manifestEntries.ByMappedLocation.TryGetValue(location, out var entry))
            {
                if (entry.Destination is not null)
                {
                    return new ContentReferenceStub(entry.Destination);
                }
            }

            return await base.ForLocationAsync(location, cancel).ConfigureAwait(false);
        }
    }
}
