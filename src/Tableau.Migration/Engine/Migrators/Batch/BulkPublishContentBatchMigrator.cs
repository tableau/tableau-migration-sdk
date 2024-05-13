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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// <see cref="IContentBatchMigrator{TContent}"/> implementation that publishes the entire batch after all items have been prepared.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public class BulkPublishContentBatchMigrator<TContent, TPublish> : ParallelContentBatchMigratorBatchBase<TContent, TPublish>
        where TContent : class, IContentReference
        where TPublish : class
    {
        private readonly IMigration _migration;
        private readonly IMigrationHookRunner _hookRunner;

        /// <summary>
        /// Creates a new <see cref="BulkPublishContentBatchMigrator{TContent, TPublish}"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="pipeline">The pipeline to use to get the item preparer.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="hookRunner">The hook runner.</param>
        public BulkPublishContentBatchMigrator(IMigration migration, IMigrationPipeline pipeline, IConfigReader configReader, IMigrationHookRunner hookRunner)
            : base(pipeline, configReader)
        {
            _migration = migration;
            _hookRunner = hookRunner;
        }

        /// <inheritdoc />
        protected override Task<IResult> MigratePreparedItemAsync(ContentMigrationItem<TContent> migrationItem, TPublish preparedItem, CancellationToken cancel)
        {
            // Always return a success, as we'll make a single bulk publish call after all items are prepared.
            var result = Result.Succeeded();
            return Task.FromResult<IResult>(result);
        }

        /// <inheritdoc />
        protected override async Task MigrateBatchAsync(ContentMigrationBatch<TContent, TPublish> batch)
        {
            // Call base class migration so we prepare all items for migration, 
            // but won't actually publish them due to overridding MigratePreparedItemAsync.
            await base.MigrateBatchAsync(batch).ConfigureAwait(false);

            // Don't publish if the batch was canceled during preparation.
            if (batch.BatchCancelSource.IsCancellationRequested)
            {
                return;
            }

            // Now publish all batch items together.
            // Batch migration doesn't return destination location information, 
            // so we cannot populate manifest destination locations.
            // Destination location fetching will be done ad-hoc by the destination caches as needed.
            var publishResult = await _migration.Destination.PublishBatchAsync(batch.PublishItems.Values, batch.BatchCancelSource.Token).ConfigureAwait(false);

            if (publishResult.Success)
            {
                batch.PublishItems.Keys.AsParallel().ForAll(i => i.ManifestEntry.SetMigrated());

                var publishedContext = new BulkPostPublishContext<TPublish>(batch.PublishItems.Values);

                await _hookRunner.ExecuteAsync<IBulkPostPublishHook<TPublish>, BulkPostPublishContext<TPublish>>(publishedContext, batch.BatchCancelSource.Token).ConfigureAwait(false);
            }
            else
            {
                batch.PublishItems.Keys.AsParallel().ForAll(i => i.ManifestEntry.SetFailed(publishResult.Errors));
            }
        }
    }

    /// <summary>
    /// <see cref="IContentBatchMigrator{TContent}"/> implementation that publishes the entire batch after all items have been prepared.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class BulkPublishContentBatchMigrator<TContent> : BulkPublishContentBatchMigrator<TContent, TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="BulkPublishContentBatchMigrator{TContent}"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="pipeline">The pipeline to use to get the item preparer.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="hookRunner">The hook runner.</param>
        public BulkPublishContentBatchMigrator(IMigration migration, IMigrationPipeline pipeline, IConfigReader configReader, IMigrationHookRunner hookRunner)
            : base(migration, pipeline, configReader, hookRunner)
        { }
    }
}
