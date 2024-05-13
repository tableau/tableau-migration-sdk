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

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Migrators
{
    /// <summary>
    /// Default <see cref="IContentMigrator{TContent}"/> implementation.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ContentMigrator<TContent> : IContentMigrator<TContent>
        where TContent : class, IContentReference
    {
        private readonly IContentBatchMigrator<TContent> _batchMigrator;
        private readonly IMigration _migration;
        private readonly IConfigReader _configReader;
        private readonly IMigrationHookRunner _hookRunner;
        private readonly IContentMappingRunner _mappingRunner;
        private readonly IContentFilterRunner _filterRunner;

        /// <summary>
        /// Creates a new <see cref="ContentMigrator{TContent}"/> object.
        /// </summary>
        /// <param name="pipeline">The pipeline to use to get the batch migrator.</param>
        /// <param name="migration">The migration being executed.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="hookRunner">The hook runner.</param>
        /// <param name="mappingRunner">The mapping runner.</param>
        /// <param name="filterRunner">The filter runner.</param>
        public ContentMigrator(
            IMigrationPipeline pipeline,
            IMigration migration,
            IConfigReader configReader,
            IMigrationHookRunner hookRunner,
            IContentMappingRunner mappingRunner,
            IContentFilterRunner filterRunner)
        {
            _batchMigrator = pipeline.GetBatchMigrator<TContent>();
            _migration = migration;
            _configReader = configReader;
            _hookRunner = hookRunner;
            _mappingRunner = mappingRunner;
            _filterRunner = filterRunner;
        }

        /// <summary>
        /// Gets the configured batch size.
        /// </summary>
        protected int BatchSize => _configReader.Get<TContent>().BatchSize;

        /// <summary>
        /// Creates a migration item context object for a given source content item and manifest entry.
        /// </summary>
        /// <param name="sourceItem">The source content item.</param>
        /// <param name="manifestEntry">The manifest entry.</param>
        /// <returns>The created migration item.</returns>
        protected static ContentMigrationItem<TContent> BuildMigrationItem(TContent sourceItem, IMigrationManifestEntryEditor manifestEntry)
            => new(sourceItem, manifestEntry);

        /// <inheritdoc />
        public async Task<IResult> MigrateAsync(CancellationToken cancel)
        {
            var resultBuilder = new ResultBuilder();
            var manifestPartition = _migration.Manifest.Entries.GetOrCreatePartition<TContent>();

            //Get the first page of source items so we know the total count, and can allocate the manifest all at once.
            var sourcePager = _migration.Source.GetPager<TContent>(BatchSize);

            var sourcePage = await sourcePager.NextPageAsync(cancel).ConfigureAwait(false);
            resultBuilder.Add(sourcePage);

            var manifestEntryBuilder = manifestPartition.GetEntryBuilder(sourcePage.TotalCount);
            while (!sourcePage.Value.IsNullOrEmpty())
            {
                var batchItems = manifestEntryBuilder.CreateEntries(sourcePage.Value, BuildMigrationItem);

                cancel.ThrowIfCancellationRequested();

                //Map all items, overwriting any mapped locations from previous attempts.
                //We do this before filtering so that filters do not see incorrect destination location/information
                //from previous runs.
                await manifestEntryBuilder.MapEntriesAsync(sourcePage.Value, _mappingRunner, cancel).ConfigureAwait(false);

                cancel.ThrowIfCancellationRequested();

                //Apply filters.
                var filteredItems = (await _filterRunner.ExecuteAsync(batchItems, cancel).ConfigureAwait(false)).ToImmutableArray();

                var skippedItems = batchItems.Except(filteredItems).ToImmutableArray();
                foreach (var skippedItem in skippedItems)
                {
                    skippedItem.ManifestEntry.SetSkipped();
                }

                cancel.ThrowIfCancellationRequested();

                //Migrate the batch.
                var batchResult = await _batchMigrator.MigrateAsync(filteredItems, cancel).ConfigureAwait(false);
                batchResult = await _hookRunner.ExecuteAsync<IContentBatchMigrationCompletedHook<TContent>, IContentBatchMigrationResult<TContent>>(batchResult, cancel).ConfigureAwait(false);

                //We only bubble up batch-level errors to the action, and not item-level errors.
                //This means a batch can succeed even if some of the items fail.
                resultBuilder.Add(batchResult);
                if (!batchResult.PerformNextBatch || sourcePage.FetchedAllPages)
                {
                    break;
                }

                cancel.ThrowIfCancellationRequested();

                //Load next page/batch to migrate.
                sourcePage = await sourcePager.NextPageAsync(cancel).ConfigureAwait(false);
                resultBuilder.Add(sourcePage);
            }

            return resultBuilder.Build();
        }
    }
}
