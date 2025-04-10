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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Manifest.Logging
{
    /// <summary>
    /// <see cref="MigrationManifestContentTypePartition"/> that writes log entries as the manifest is manipulated.
    /// </summary>
    public class LoggingMigrationManifestContentTypePartition : MigrationManifestContentTypePartition
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<MigrationManifestContentTypePartition> _logger;

        /// <summary>
        /// Creates a new <see cref="LoggingMigrationManifestContentTypePartition"/> object.
        /// </summary>
        /// <param name="type">The content type the partition holds manifest entries for.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="logger">The logger.</param>
        public LoggingMigrationManifestContentTypePartition(Type type, ISharedResourcesLocalizer localizer, ILogger<MigrationManifestContentTypePartition> logger)
            : base(type)
        {
            _localizer = localizer;
            _logger = logger;
        }

        /// <inheritdoc />
        public override void MigrationFailed(IMigrationManifestEntryEditor entry)
        {
            foreach (var error in entry.Errors)
            {
                _logger.LogError(_localizer[SharedResourceKeys.MigrationItemErrorLogMessage], ContentType, entry.Source.Location, error, error.Data.GetContentsAsString());
            }
        }
    }
}
