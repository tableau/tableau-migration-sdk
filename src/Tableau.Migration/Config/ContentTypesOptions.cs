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
            /// The default migration overwrite group users flag.
            /// </summary>
            public const bool OVERWRITE_GROUP_USERS_ENABLED = true;

            /// <summary>
            /// The default migration overwrite user favorites flag.
            /// </summary>
            public const bool OVERWRITE_USER_FAVORITES_ENABLED = true;

            /// <summary>
            /// The default overwrite group set groups flag.
            /// </summary>
            public const bool OVERWRITE_GROUP_SET_GROUPS = true;
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
        /// Gets or sets the overwrite group users flag for supported types. Default: enabled.<br/>
        /// <b>Important:</b> This option is only available to <see cref="Content.IGroup"/>.
        /// For more details, check the configuration 
        /// <seealso href="https://tableau.github.io/migration-sdk/articles/configuration.html#contenttypesoverwritegroupusersenabled">article</seealso>.
        /// </summary>
        public bool OverwriteGroupUsersEnabled
        {
            get => _overwriteGroupUsersEnabled ?? Defaults.OVERWRITE_GROUP_USERS_ENABLED;
            set => _overwriteGroupUsersEnabled = value;
        }
        private bool? _overwriteGroupUsersEnabled;

        /// <summary>
        /// When true, destination favorites will be deleted for all migrated users as a user post-publish hook. This will delete favorites that exist on the destination but are not present on the source.
        /// <b>Important:</b> This option is only available for the <see cref="Content.IFavorite"/> content type.
        /// </summary>
        public bool OverwriteUserFavoritesEnabled
        {
            get => _overwriteUserFavoritesEnabled ?? Defaults.OVERWRITE_USER_FAVORITES_ENABLED;
            set => _overwriteUserFavoritesEnabled = value;
        }
        private bool? _overwriteUserFavoritesEnabled;

        /// <summary>
        /// Gets or sets the overwrite group set groups flag for supported types. Default: enabled.<br/>
        /// <b>Important:</b> This option is only available to <see cref="Content.IGroupSet"/>.
        /// For more details, check the configuration 
        /// <seealso href="https://tableau.github.io/migration-sdk/articles/configuration.html#contenttypesoverwritegroupsetgroupsenabled">article</seealso>.
        /// </summary>
        public bool OverwriteGroupSetGroupsEnabled
        {
            get => _overwriteGroupSetGroupsEnabled ?? Defaults.OVERWRITE_GROUP_SET_GROUPS;
            set => _overwriteGroupSetGroupsEnabled = value;
        }
        private bool? _overwriteGroupSetGroupsEnabled;

        /// <summary>
        /// Checks if the content type in <see cref="Type"/> is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsContentTypeValid()
            => MigrationPipelineContentType.GetAllMigrationPipelineContentTypes()
                .Any(c => string.Equals(c.GetConfigKey(), Type, StringComparison.OrdinalIgnoreCase));
    }
}