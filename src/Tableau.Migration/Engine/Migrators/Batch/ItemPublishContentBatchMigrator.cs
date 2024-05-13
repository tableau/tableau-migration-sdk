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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// <see cref="IContentBatchMigrator{TContent}"/> implementation that publishes items one-by-one.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    /// <typeparam name="TResult">The post-publish result type.</typeparam>
    public class ItemPublishContentBatchMigrator<TContent, TPublish, TResult> : ParallelContentBatchMigratorBatchBase<TContent, TPublish>
        where TContent : class, IContentReference
        where TPublish : class
        where TResult : class, IContentReference
    {
        private readonly IMigration _migration;
        private readonly IMigrationHookRunner _hookRunner;

        /// <summary>
        /// Creates a new <see cref="ItemPublishContentBatchMigrator{TContent, TPublish, TResult}"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="pipeline">The pipeline to use to get the item preparer.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="hookRunner">The hook runner.</param>
        public ItemPublishContentBatchMigrator(IMigration migration, IMigrationPipeline pipeline, IConfigReader configReader, IMigrationHookRunner hookRunner)
            : base(pipeline, configReader)
        {
            _migration = migration;
            _hookRunner = hookRunner;
        }

        /// <inheritdoc />
        protected override async Task<IResult> MigratePreparedItemAsync(ContentMigrationItem<TContent> migrationItem, TPublish preparedItem, CancellationToken cancel)
        {
            cancel.ThrowIfCancellationRequested();

            var manifestEntry = migrationItem.ManifestEntry;

            var publishResult = await _migration.Destination.PublishAsync<TPublish, TResult>(preparedItem, cancel).ConfigureAwait(false);
            if (!publishResult.Success)
            {
                return publishResult;
            }

            manifestEntry.DestinationFound(publishResult.Value);
            manifestEntry.SetMigrated();

            var publishedContext = new ContentItemPostPublishContext<TPublish, TResult>(manifestEntry, preparedItem, publishResult.Value);

            await _hookRunner.ExecuteAsync<IContentItemPostPublishHook<TPublish, TResult>, ContentItemPostPublishContext<TPublish, TResult>>(publishedContext, cancel).ConfigureAwait(false);

            return publishResult;
        }
    }

    /// <summary>
    /// <see cref="IContentBatchMigrator{TContent}"/> implementation that publishes items one-by-one.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc /></typeparam>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    public class ItemPublishContentBatchMigrator<TContent, TPublish> : ItemPublishContentBatchMigrator<TContent, TPublish, TContent>
        where TContent : class, IContentReference
        where TPublish : class
    {
        /// <summary>
        /// Creates a new <see cref="ItemPublishContentBatchMigrator{TContent}"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="pipeline">The pipeline to use to get the item preparer.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="hookRunner">The hook runner.</param>
        public ItemPublishContentBatchMigrator(IMigration migration, IMigrationPipeline pipeline, IConfigReader configReader, IMigrationHookRunner hookRunner)
            : base(migration, pipeline, configReader, hookRunner)
        { }
    }

    /// <summary>
    /// <see cref="IContentBatchMigrator{TContent}"/> implementation that publishes items one-by-one.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ItemPublishContentBatchMigrator<TContent> : ItemPublishContentBatchMigrator<TContent, TContent, TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="ItemPublishContentBatchMigrator{TContent}"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="pipeline">The pipeline to use to get the item preparer.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="hookRunner">The hook runner.</param>
        public ItemPublishContentBatchMigrator(IMigration migration, IMigrationPipeline pipeline, IConfigReader configReader, IMigrationHookRunner hookRunner)
            : base(migration, pipeline, configReader, hookRunner)
        { }
    }
}
