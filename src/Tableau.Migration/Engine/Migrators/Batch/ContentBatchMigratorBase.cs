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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Engine.Preparation;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// Abstract base class for <see cref="IContentBatchMigrator{TContent}"/> implementations.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public abstract class ContentBatchMigratorBase<TContent, TPublish> : IContentBatchMigrator<TContent>
        where TContent : class, IContentReference
        where TPublish : class
    {
        private readonly IContentItemPreparer<TContent, TPublish> _itemPreparer;

        /// <summary>
        /// Creates a new <see cref="ContentBatchMigratorBase{TContent, TPublish}"/> object.
        /// </summary>
        /// <param name="pipeline">The migration pipeline.</param>
        public ContentBatchMigratorBase(IMigrationPipeline pipeline)
        {
            _itemPreparer = pipeline.GetItemPreparer<TContent, TPublish>();
        }

        /// <summary>
        /// Migrates the batch of content items.
        /// </summary>
        /// <param name="batch">The batch to migrate.</param>
        protected abstract Task MigrateBatchAsync(ContentMigrationBatch<TContent, TPublish> batch);

        /// <summary>
        /// Performs post-preparation item migration (e.g. publishing, adding to a bulk import, etc.).
        /// </summary>
        /// <param name="migrationItem">The item being migrated.</param>
        /// <param name="preparedItem">The prepared publish item.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the migration.</returns>
        protected abstract Task<IResult> MigratePreparedItemAsync(ContentMigrationItem<TContent> migrationItem, TPublish preparedItem, CancellationToken cancel);

        /// <summary>
        /// Migrates a single content item.
        /// </summary>
        /// <param name="item">The item to migrate.</param>
        /// <param name="batch">The batch being migrated.</param>
        /// <returns>A task to await.</returns>
        protected virtual async Task MigrateBatchItemAsync(ContentMigrationItem<TContent> item, ContentMigrationBatch<TContent, TPublish> batch)
        {
            var itemCancel = batch.BatchCancelSource.Token;

            itemCancel.ThrowIfCancellationRequested();

            //Since this may be done in parallel we catch exceptions here to prevent unobserved task exceptions in naive item migrator implementations.
            IContentItemMigrationResult<TContent> itemResult;
            try
            {
                var prepareResult = await _itemPreparer.PrepareAsync(item, itemCancel).ConfigureAwait(false);
                if (prepareResult.Success)
                {
                    var publishItem = prepareResult.Value;
                    batch.PublishItems.AddOrUpdate(item, publishItem, (i, p) => publishItem);

                    var migrateResult = await MigratePreparedItemAsync(item, publishItem, itemCancel).ConfigureAwait(false);

                    itemResult = ContentItemMigrationResult<TContent>.FromResult(migrateResult, item.ManifestEntry);
                }
                else
                {
                    itemResult = ContentItemMigrationResult<TContent>.Failed(item.ManifestEntry, prepareResult.Errors);
                }

                itemCancel.ThrowIfCancellationRequested();
            }
            catch (Exception ex) when (ex.IsCancellationException())
            {
                item.ManifestEntry.SetCanceled();
                itemResult = ContentItemMigrationResult<TContent>.Canceled(item.ManifestEntry, new[] { ex }, false);
            }
            catch (Exception ex) when (!ex.IsCancellationException())
            {
                itemResult = ContentItemMigrationResult<TContent>.Failed(item.ManifestEntry, new[] { ex });
            }

            batch.ItemResults.Enqueue(itemResult);

            if (!itemResult.Success && !itemResult.IsCanceled)
            {
                item.ManifestEntry.SetFailed(itemResult.Errors);
            }

            if (!itemResult.ContinueBatch)
            {
                batch.BatchCancelSource.Cancel();
            }
        }

        /// <inheritdoc />
        public async Task<IContentBatchMigrationResult<TContent>> MigrateAsync(ImmutableArray<ContentMigrationItem<TContent>> itemBatch, CancellationToken migrationCancel)
        {
            var batch = new ContentMigrationBatch<TContent, TPublish>(itemBatch, migrationCancel);
            await using (batch)
            {
                try
                {
                    await MigrateBatchAsync(batch).ConfigureAwait(false);
                }
                //Catch cancellation since it might have been only for this batch, not the migration as a whole.
                catch (Exception ex) when (ex.IsCancellationException())
                {
                    //Ensure items we didn't get to before the cancellation token threw are set as canceled.
                    itemBatch
                        .Where(i => i.ManifestEntry.Status == Manifest.MigrationManifestEntryStatus.Pending)
                        .AsParallel()
                        .ForAll(i => i.ManifestEntry.SetCanceled());

                    //Rethrow if this was a migration-level cancellation, don't rethrow if it was a batch-level cancellation.
                    migrationCancel.ThrowIfCancellationRequested();
                }

                //Default behavior is to not bubble up errors migrating individual items onto the main migration result, 
                //as we don't want to confuse users that the entire migration failed.
                //Users can look at the manifest to find specific items that had issues.
                return ContentBatchMigrationResult<TContent>.Succeeded(batch.ItemResults.ToImmutableArray());
            }
        }
    }

    /// <summary>
    /// Abstract base class for <see cref="IContentBatchMigrator{TContent}"/> implementations.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public abstract class ContentBatchMigratorBase<TContent> : ContentBatchMigratorBase<TContent, TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="ContentBatchMigratorBase{TContent}"/> object.
        /// </summary>
        /// <param name="pipeline">The migration pipeline.</param>
        public ContentBatchMigratorBase(
            IMigrationPipeline pipeline)
            : base(pipeline)
        { }
    }
}
