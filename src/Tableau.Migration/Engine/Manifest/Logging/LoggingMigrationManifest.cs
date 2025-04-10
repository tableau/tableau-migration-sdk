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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Manifest.Logging
{
    /// <summary>
    /// Migration manifest that writes log entries as the manifest is manipulated.
    /// </summary>
    public class LoggingMigrationManifest : MigrationManifest
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<MigrationManifest> _logger;

        /// <summary>
        /// Creates a new <see cref="LoggingMigrationManifest"/> object.
        /// </summary>
        /// <param name="localizer">The localizer.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="planId"><inheritdoc cref="IMigrationManifest.PlanId"/></param>
        /// <param name="migrationId"><inheritdoc cref="IMigrationManifest.MigrationId"/></param>
        /// <param name="copyEntriesManifest">
        /// An optional manifest to copy entries from.
        /// Null will initialize the manifest with an empty set of entries. 
        /// Top-level information is not copied.
        /// </param>
        public LoggingMigrationManifest(
            ISharedResourcesLocalizer localizer,
            ILoggerFactory loggerFactory,
            Guid planId,
            Guid migrationId,
            IMigrationManifest copyEntriesManifest)
            : base(planId, migrationId, copyEntriesManifest.PipelineProfile, copyEntriesManifest)
        {
            _localizer = localizer;
            _logger = loggerFactory.CreateLogger<LoggingMigrationManifest>();
        }

        /// <summary>
        /// Creates a new <see cref="LoggingMigrationManifest"/> object.
        /// </summary>
        /// <param name="localizer">The localizer.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="planId"><inheritdoc cref="IMigrationManifest.PlanId"/></param>
        /// <param name="migrationId"><inheritdoc cref="IMigrationManifest.MigrationId"/></param>
        /// <param name="pipelineProfile"></param>
        public LoggingMigrationManifest(
            ISharedResourcesLocalizer localizer,
            ILoggerFactory loggerFactory,
            Guid planId,
            Guid migrationId,
            PipelineProfile pipelineProfile
            )
            : base(planId, migrationId, pipelineProfile)
        {
            _localizer = localizer;
            _logger = loggerFactory.CreateLogger<LoggingMigrationManifest>();
        }

        /// <inheritdoc />
        public override IMigrationManifestEditor AddErrors(params IEnumerable<Exception> errors)
        {
            foreach (var error in errors)
            {
                _logger.LogError(_localizer[SharedResourceKeys.MigrationErrorLogMessage], error);
            }

            return base.AddErrors(errors);
        }

        /// <inheritdoc />
        public override IMigrationManifestEditor AddErrors(params Exception[] errors)
            => AddErrors((IEnumerable<Exception>)errors); //Overload for Python interop.
    }
}
