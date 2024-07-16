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

using System;
using System.Linq;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Class for configuration settings specific to content types.
    /// </summary>
    public class ContentTypesOptions
    {
        /// <summary>
        /// Defaults for migration options.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default migration batch size. 
            /// </summary>
            public const int BATCH_SIZE = 100;

            /// <summary>
            /// The default migration batch publishing flag.
            /// </summary>
            public const bool BATCH_PUBLISHING_ENABLED = false;

            /// <summary>
            /// The default migration include extract flag
            /// </summary>
            public const bool INCLUDE_EXTRACT_ENABLED = true;
        }

        /// <summary>
        /// The name of the content type Ex: User.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the migration batch size.
        /// </summary>
        public int BatchSize
        {
            get => _batchSize ?? Defaults.BATCH_SIZE;
            set => _batchSize = value;
        }
        private int? _batchSize;

        /// <summary>
        /// Gets or sets the batch publishing to the supported types. Default: disabled.<br/>
        /// <b>Important:</b> This option is only available to <see cref="Content.IUser"/>.
        /// For more details, check the configuration <seealso href="https://tableau.github.io/migration-sdk/articles/configuration.html#contenttypesbatchpublishingenabled">article</seealso>.
        /// </summary>
        public bool BatchPublishingEnabled
        {
            get => _batchPublishingEnabled ?? Defaults.BATCH_PUBLISHING_ENABLED;
            set => _batchPublishingEnabled = value;
        }
        private bool? _batchPublishingEnabled;
        
        /// <summary>
        /// Gets or sets the include extract flag for supported types. Default: enabled.<br/>
        /// <b>Important:</b> This option is only available to <see cref="Content.IWorkbook"/> and <see cref="Content.IDataSource"/>.
        /// For more details, check the configuration <seealso href="https://tableau.github.io/migration-sdk/articles/configuration.html#contenttypesincludeextractenabled">article</seealso>.
        /// </summary>
        public bool IncludeExtractEnabled
        {
            get => _includeExtractEnabled ?? Defaults.INCLUDE_EXTRACT_ENABLED;
            set => _includeExtractEnabled = value;
        }
        private bool? _includeExtractEnabled;

        /// <summary>
        /// Checks if the content type in <see cref="Type"/> is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsContentTypeValid() 
            => ServerToCloudMigrationPipeline
                .ContentTypes
                .Any(c => string.Equals(c.GetConfigKey(), Type, StringComparison.OrdinalIgnoreCase));
    }
}