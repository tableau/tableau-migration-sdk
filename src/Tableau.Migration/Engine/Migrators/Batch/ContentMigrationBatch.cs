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
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// Class representing the state of a batch of content items being migrated.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public class ContentMigrationBatch<TContent, TPublish> : IAsyncDisposable
    {
        /// <summary>
        /// Gets the items in the batch.
        /// </summary>
        public ImmutableArray<ContentMigrationItem<TContent>> Items { get; }

        /// <summary>
        /// Gets the batch-level cancellation source, linked to migration cancellation token.
        /// </summary>
        public CancellationTokenSource BatchCancelSource { get; }

        /// <summary>
        /// Gets the collection of item-level migration results.
        /// </summary>
        public ConcurrentQueue<IContentItemMigrationResult<TContent>> ItemResults { get; }

        /// <summary>
        /// Gets the collection of items to publish.
        /// </summary>
        public ConcurrentDictionary<ContentMigrationItem<TContent>, TPublish> PublishItems { get; }

        /// <summary>
        /// Creates a new <see cref="ContentMigrationBatch{TContent, TPublish}"/> object.
        /// </summary>
        /// <param name="items">The items in the batch.</param>
        /// <param name="migrationCancel">A migration-level cancellation token to gnerate a batch cancellation source from.</param>
        public ContentMigrationBatch(ImmutableArray<ContentMigrationItem<TContent>> items, CancellationToken migrationCancel)
        {
            Items = items;

            //Create a linked cancellation token so we automatically cancel the batch if the main cancellation token cancels, 
            //but can also cancel the batch without cancelling the entire migration.            
            BatchCancelSource = CancellationTokenSource.CreateLinkedTokenSource(migrationCancel);

            ItemResults = new();
            PublishItems = new();
        }

        #region - IAsyncDisposable Implementation -

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public virtual async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            foreach (var publishItem in PublishItems.Values)
            {
                await publishItem.DisposeIfNeededAsync().ConfigureAwait(false);
            }

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
