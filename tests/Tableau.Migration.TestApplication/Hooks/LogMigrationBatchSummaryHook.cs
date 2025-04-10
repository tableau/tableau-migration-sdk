//
//  Copyright (c) 2025, Salesforce, Inc.
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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.TestApplication.Hooks
{
    internal sealed class LogMigrationBatchSummaryHook<T> : IContentBatchMigrationCompletedHook<T>
        where T : IContentReference
    {
        private readonly IMigrationManifest _manifest;
        private readonly ILogger<LogMigrationBatchSummaryHook<T>> _logger;

        public LogMigrationBatchSummaryHook(IMigrationManifest manifest,
            ILogger<LogMigrationBatchSummaryHook<T>> logger)
        {
            _manifest = manifest;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<IContentBatchMigrationResult<T>?> ExecuteAsync(IContentBatchMigrationResult<T> ctx, CancellationToken cancel)
        {
            var entries = _manifest.Entries.ForContentType<T>();

            var contentTypeName = MigrationPipelineContentType.GetConfigKeyForType(typeof(T));

            var processedCount = entries.GetStatusTotals()
                .Where(s => s.Key is not MigrationManifestEntryStatus.Pending)
                .Sum(s => s.Value);

            _logger.LogInformation("{ContentType} batch completed for {Count} non-skipped item(s). Total processed: {ProcessedCount} / {Total}",
                contentTypeName, ctx.ItemResults.Count, processedCount, entries.ExpectedTotalCount);

            return Task.FromResult<IContentBatchMigrationResult<T>?>(ctx);
        }
    }
}
