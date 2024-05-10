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

using System.Collections.Generic;

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Configuration options for the <see cref="Migration"/> SDK.
    /// </summary>
    public class MigrationSdkOptions
    {
        /// <summary>
        /// Defaults for migration options.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default number of items to migrate in parallel.
            /// </summary>
            public const int MIGRATION_PARALLELISM = 10;
        }

        /// <summary>
        /// Get or Set content type specific options.
        /// </summary>      
        public List<ContentTypesOptions> ContentTypes { get; set; } = new();

        /// <summary>
        /// Gets or sets the number of items to migrate in parallel.
        /// </summary>
        public int MigrationParallelism
        {
            get => _migrationParallelism ?? Defaults.MIGRATION_PARALLELISM;
            set => _migrationParallelism = value;
        }
        private int? _migrationParallelism;

        /// <summary>
        /// Gets or sets options related to file storage.
        /// </summary>
        public FileOptions Files { get; set; } = new();

        /// <summary>
        /// Gets or sets options related to Tableau connections.
        /// </summary>
        public NetworkOptions Network { get; set; } = new();

        /// <summary>
        /// Default project permissions content types.
        /// </summary>
        public DefaultPermissionsContentTypeOptions DefaultPermissionsContentTypes { get; set; } = new();

        /// <summary>
        /// Gets or sets options related to jobs.
        /// </summary>
        public JobOptions Jobs { get; set; } = new();
    }
}