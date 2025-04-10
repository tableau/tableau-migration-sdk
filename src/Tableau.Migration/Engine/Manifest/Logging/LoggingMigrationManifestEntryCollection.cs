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
    /// <see cref="MigrationManifestEntryCollection"/> that writes log entries as the manifest is manipulated.
    /// </summary>
    public class LoggingMigrationManifestEntryCollection : MigrationManifestEntryCollection
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<MigrationManifestContentTypePartition> _partitionLogger;

        /// <summary>
        /// Creates a new <see cref="LoggingMigrationManifestEntryCollection"/> object.
        /// </summary>
        /// <param name="localizer">The localizer.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="copy">An optional collection to deep copy entries from.</param>
        public LoggingMigrationManifestEntryCollection(ISharedResourcesLocalizer localizer, ILoggerFactory loggerFactory, IMigrationManifestEntryCollection? copy = null)
            : base(copy)
        {
            _localizer = localizer;
            _partitionLogger = loggerFactory.CreateLogger<MigrationManifestContentTypePartition>();
        }

        /// <inheritdoc />
        protected override MigrationManifestContentTypePartition CreateParition(Type contentType)
            => new LoggingMigrationManifestContentTypePartition(contentType, _localizer, _partitionLogger);
    }
}
