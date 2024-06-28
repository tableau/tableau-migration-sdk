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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Hooks.Mappings;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Interface for an object that can create new or link existing manifest entries to source content items.
    /// </summary>
    public interface IMigrationManifestEntryBuilder
    {
        /// <summary>
        /// Creates or links manifest entries to a set of content items.
        /// </summary>
        /// <typeparam name="TItem">The input content item type to use.</typeparam>
        /// <typeparam name="TResultItem">The result item to return.</typeparam>
        /// <param name="sourceContentItems">The source content items to create or link manifest entries for.</param>
        /// <param name="resultFactory">A factory function to produce result items for, useful for linking a created manifest entry with the source content item it is associated with.</param>
        /// <returns>An immutable array of results returned by <paramref name="resultFactory" /> for each new entry.</returns>
        ImmutableArray<TResultItem> CreateEntries<TItem, TResultItem>(IReadOnlyCollection<TItem> sourceContentItems,
            Func<TItem, IMigrationManifestEntryEditor, TResultItem> resultFactory)
            where TItem : IContentReference;

        /// <summary>
        /// Maps all existing manifest entries to their intended destination locations.
        /// </summary>
        /// <typeparam name="TItem">The input content item type.</typeparam>
        /// <param name="sourceContentItems">The content items to map.</param>
        /// <param name="mapper">An object to use to map entries. Supplied as a parameter to avoid DI circular references at plan initialization time.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The current entry builder, for fluent API usage.</returns>
        Task<IMigrationManifestEntryBuilder> MapEntriesAsync<TItem>(IEnumerable<TItem> sourceContentItems, IContentMappingRunner mapper, CancellationToken cancel)
            where TItem : IContentReference;

        /// <summary>
        /// Registers a destination information update for an entry to update caches.
        /// </summary>
        /// <param name="entry">The manifest entry that was updated.</param>
        /// <param name="oldDestinationInfo">The old destination information.</param>
        void DestinationInfoUpdated(IMigrationManifestEntryEditor entry, IContentReference? oldDestinationInfo);

        /// <summary>
        /// Registers that migration failed for a content item for logging.
        /// </summary>
        /// <param name="entry">The manifest entry that was updated.</param>
        void MigrationFailed(IMigrationManifestEntryEditor entry);
    }
}
