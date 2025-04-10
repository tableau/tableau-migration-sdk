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
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Migrators.Batch;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// <see cref="IMigrationPipeline"/> implementation to perform migrations from Tableau Server to Tableau Server.
    /// </summary>
    public class ServerToServerMigrationPipeline : MigrationPipelineBase
    {
        /// <summary>
        /// Content types that are supported for migrations.
        /// </summary>
        public static readonly ImmutableArray<MigrationPipelineContentType> ContentTypes =
        [
            MigrationPipelineContentType.Users,
            MigrationPipelineContentType.Groups,
            MigrationPipelineContentType.Projects,
            MigrationPipelineContentType.DataSources,
            MigrationPipelineContentType.Workbooks,
            MigrationPipelineContentType.ServerToServerExtractRefreshTasks,
            MigrationPipelineContentType.CustomViews,
            MigrationPipelineContentType.ServerToServerSubscriptions
        ];

        /// <summary>
        /// Creates a new <see cref="ServerToServerMigrationPipeline"/> object.
        /// </summary>
        /// <param name="services"><inheritdoc /></param>
        /// <param name="configReader">A config reader to get the REST API configuration.</param>
        public ServerToServerMigrationPipeline(IServiceProvider services,
            IConfigReader configReader)
            : base(services, configReader)
        { }


        /// <inheritdoc />
        protected override IEnumerable<IMigrationAction> BuildPipeline()
        {
            yield return CreateAction<PreflightAction>();

            //Migrate users and groups first since many content types depend on them,
            //We migrate users before groups because group membership must use 
            //per-user or per-group requests, and we assume in most cases
            //there will be less groups than users.
            yield return CreateMigrateContentAction<IUser>();
            yield return CreateMigrateContentAction<IGroup>();
            yield return CreateMigrateContentAction<IProject>();
            yield return CreateMigrateContentAction<IDataSource>();
            yield return CreateMigrateContentAction<IWorkbook>();
            yield return CreateMigrateContentAction<IServerExtractRefreshTask>();
            yield return CreateMigrateContentAction<ICustomView>();
            yield return CreateMigrateContentAction<IServerSubscription>();
        }

        /// <inheritdoc />
        public override IContentBatchMigrator<TContent> GetBatchMigrator<TContent>()
        {
            switch (typeof(TContent))
            {
                case Type extractRefreshTask when extractRefreshTask == typeof(IServerExtractRefreshTask):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();

                case Type subscription when subscription == typeof(IServerSubscription):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();

                default:
                    return base.GetBatchMigrator<TContent>();
            }
        }
    }
}
