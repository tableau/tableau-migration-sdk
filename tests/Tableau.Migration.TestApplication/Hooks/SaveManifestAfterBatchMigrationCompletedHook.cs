﻿//
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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.TestApplication.Config;
using Tableau.Migration.TestComponents.Engine.Manifest;

namespace Tableau.Migration.TestApplication.Hooks
{
    /// <summary>
    /// Saves manifest after every content type
    /// </summary>
    public class SaveManifestAfterBatchMigrationCompletedHook<T> : IContentBatchMigrationCompletedHook<T> where T : IContentReference
    {
        private readonly IMigration _migration;
        private readonly MigrationManifestSerializer _manifestSerializer;
        private readonly TestApplicationOptions _options;
        private readonly string _manifestFilepath;
        private readonly ILogger<SaveManifestAfterBatchMigrationCompletedHook<T>> _logger;

        public SaveManifestAfterBatchMigrationCompletedHook(
            IMigration migration,
            MigrationManifestSerializer manifestSerializer,
            IOptions<TestApplicationOptions> options,
            ILogger<SaveManifestAfterBatchMigrationCompletedHook<T>> logger)
        {
            _migration = migration;
            _manifestSerializer = manifestSerializer;
            _options = options.Value;
            _logger = logger;

            _manifestFilepath = $@"{_options.Log.FolderPath}\Manifest-{Program.StartTime.ToString("yyyy-MM-dd-HH-mm-ss")}.json";
        }

        public async Task<Engine.Migrators.Batch.IContentBatchMigrationResult<T>?> ExecuteAsync(Engine.Migrators.Batch.IContentBatchMigrationResult<T> ctx, CancellationToken cancel)
        {
            _logger.LogDebug("Saving manifest");
            
            // Using default cancellation token because we want to save the manifest even if app is shutting down
            await _manifestSerializer.SaveAsync(_migration.Manifest, _manifestFilepath); 

            return ctx;
        }
    }
}
