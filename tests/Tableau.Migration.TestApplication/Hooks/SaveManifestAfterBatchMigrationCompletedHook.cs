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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication.Hooks
{
    /// <summary>
    /// Saves manifest after every content type
    /// </summary>
    public class SaveManifestAfterBatchMigrationCompletedHook<T> : IContentBatchMigrationCompletedHook<T> where T : IContentReference
    {
        private readonly IMigration _migration;
        private readonly MigrationManifestSerializer _manifestSerializer;
        private readonly string _manifestFilePath;
        private readonly ILogger<SaveManifestAfterBatchMigrationCompletedHook<T>> _logger;

        public SaveManifestAfterBatchMigrationCompletedHook(
            IMigration migration,
            MigrationManifestSerializer manifestSerializer,
            IOptions<TestApplicationOptions> options,
            ILogger<SaveManifestAfterBatchMigrationCompletedHook<T>> logger)
        {
            _migration = migration;
            _manifestSerializer = manifestSerializer;
            _logger = logger;
            _manifestFilePath = LogFileHelper.GetManifestFilePath(options.Value.Log);
        }

        public async Task<Engine.Migrators.Batch.IContentBatchMigrationResult<T>?> ExecuteAsync(Engine.Migrators.Batch.IContentBatchMigrationResult<T> ctx, CancellationToken cancel)
        {
            _logger.LogDebug("Saving manifest");

            // Using default cancellation token because we want to save the manifest even if app is shutting down
            await _manifestSerializer.SaveAsync(_migration.Manifest, _manifestFilePath);

            return ctx;
        }
    }
}
