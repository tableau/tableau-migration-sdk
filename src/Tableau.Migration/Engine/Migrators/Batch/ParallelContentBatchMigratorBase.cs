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

using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// Abstract base class for <see cref="IContentBatchMigrator{TContent}" /> implementations that migrates items in parallel.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public abstract class ParallelContentBatchMigratorBatchBase<TContent, TPublish> : ContentBatchMigratorBase<TContent, TPublish>
        where TContent : class, IContentReference
        where TPublish : class
    {
        private readonly IConfigReader _configReader;

        /// <summary>
        /// Creates a new <see cref="ParallelContentBatchMigratorBatchBase{TContent, TPublish}"/> object.
        /// </summary>
        /// <param name="pipeline">The pipeline to use to get the item preparer.</param>
        /// <param name="configReader">The configuration reader.</param>
        public ParallelContentBatchMigratorBatchBase(IMigrationPipeline pipeline, IConfigReader configReader)
            : base(pipeline)
        {
            _configReader = configReader;
        }

        /// <inheritdoc />
        protected override async Task MigrateBatchAsync(ContentMigrationBatch<TContent, TPublish> batch)
        {
            var opts = new ParallelOptions
            {
                CancellationToken = batch.BatchCancelSource.Token,
                MaxDegreeOfParallelism = _configReader.Get().MigrationParallelism
            };

            //We use Parallel.ForEachAsync instead of AsParallel.ForAll because each item being processed will likely incur
            //IO blocking due to API calls, so the overhead of more thread synchronization is the lesser of evils.
            await Parallel.ForEachAsync(batch.Items, opts, async (item, itemCancel) =>
            {
                await base.MigrateBatchItemAsync(item, batch).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Abstract base class for <see cref="IContentBatchMigrator{TContent}" /> implementations that migrates items in parallel.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public abstract class ParallelContentBatchMigratorBatchBase<TContent> : ParallelContentBatchMigratorBatchBase<TContent, TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="ParallelContentBatchMigratorBatchBase{TContent}"/> object.
        /// </summary>
        /// <param name="pipeline">The pipeline to use to get the item preparer.</param>
        /// <param name="configReader">The configuration reader.</param>
        protected ParallelContentBatchMigratorBatchBase(IMigrationPipeline pipeline, IConfigReader configReader)
            : base(pipeline, configReader)
        { }
    }
}
