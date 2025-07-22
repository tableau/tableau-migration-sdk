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
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Migrators.Batch;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// <see cref="IMigrationPipeline"/> implementation to perform migrations from Tableau Cloud to Tableau Cloud.
    /// </summary>
    public class CloudToCloudMigrationPipeline : TableauMigrationPipelineBase
    {
        /// <summary>
        /// Content types that are supported for migrations.
        /// </summary>
        public static readonly ImmutableArray<MigrationPipelineContentType> ContentTypes =
        [
            MigrationPipelineContentType.Users,
            MigrationPipelineContentType.Groups,
            MigrationPipelineContentType.GroupSets,
            MigrationPipelineContentType.Projects,
            MigrationPipelineContentType.DataSources,
            MigrationPipelineContentType.Workbooks,
            MigrationPipelineContentType.CloudToCloudExtractRefreshTasks,
            MigrationPipelineContentType.CustomViews,
            MigrationPipelineContentType.CloudToCloudSubscriptions,
            MigrationPipelineContentType.Favorites
        ];

        /// <summary>
        /// Creates a new <see cref="CloudToCloudMigrationPipeline"/> object.
        /// </summary>
        /// <param name="services"><inheritdoc /></param>
        /// <param name="configReader">A config reader to get the REST API configuration.</param>
        public CloudToCloudMigrationPipeline(IServiceProvider services,
            IConfigReader configReader)
            : base(services, configReader)
        { }

        /// <inheritdoc />
        protected override IMigrationAction CreateExtractRefreshTaskAction()
            => CreateMigrateContentAction<ICloudExtractRefreshTask>();

        /// <inheritdoc />
        protected override IMigrationAction CreateSubscriptionAction()
            => CreateMigrateContentAction<ICloudSubscription>();

        /// <inheritdoc />
        public override IContentBatchMigrator<TContent> GetBatchMigrator<TContent>()
        {
            switch (typeof(TContent))
            {
                case Type extractRefreshTask when extractRefreshTask == typeof(ICloudExtractRefreshTask):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();

                case Type subscription when subscription == typeof(ICloudSubscription):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();

                default:
                    return base.GetBatchMigrator<TContent>();
            }
        }
    }
}
