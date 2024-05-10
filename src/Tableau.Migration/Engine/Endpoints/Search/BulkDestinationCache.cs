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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="BulkApiContentReferenceCache{TContent}"/> implementation
    /// that falls back to bulk API listing when destination information is not found in the manifest.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class BulkDestinationCache<TContent> : BulkApiContentReferenceCache<TContent>
        where TContent : class, IContentReference
    {
        private readonly IMigrationManifestContentTypePartitionEditor _manifestEntries;

        /// <summary>
        /// Creates a new <see cref="BulkDestinationCache{TContent}"/>
        /// </summary>
        /// <param name="endpoint">The destination endpoint.</param>
        /// <param name="configReader">A config reader.</param>
        /// <param name="manifest">A migration manifest.</param>
        public BulkDestinationCache(
            IDestinationEndpoint endpoint,
            IConfigReader configReader, 
            IMigrationManifestEditor manifest)
            : base((endpoint as IDestinationApiEndpoint)?.SiteApi, configReader)
        {
            _manifestEntries = manifest.Entries.GetOrCreatePartition<TContent>();
        }

        /// <inheritdoc />
        protected override void ItemLoaded(TContent item)
        {
            //Assign this info to the manifest if there's an entry with our mapped location.
            //This updates any ID/other information that may have changed since last run.
            if (_manifestEntries.ByMappedLocation.TryGetValue(item.Location, out var manifestEntry))
            {
                manifestEntry.DestinationFound(new ContentReferenceStub(item));
            }
            base.ItemLoaded(item);
        }

        /// <inheritdoc />
        protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(ContentLocation searchLocation, CancellationToken cancel)
        {
            if (_manifestEntries.ByMappedLocation.TryGetValue(searchLocation, out var entry))
            {
                if (entry.Destination is not null)
                {
                    return new[] { new ContentReferenceStub(entry.Destination) };
                }
            }

            return await base.SearchAsync(searchLocation, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(Guid searchId, CancellationToken cancel)
        {
            if (_manifestEntries.ByDestinationId.TryGetValue(searchId, out var entry))
            {
                if (entry.Destination is not null)
                {
                    return new[] { new ContentReferenceStub(entry.Destination) };
                }
            }

            return await base.SearchAsync(searchId, cancel).ConfigureAwait(false);
        }
    }
}
