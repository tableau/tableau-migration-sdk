// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Migrators.Batch;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// <see cref="IMigrationPipeline"/> implementation to perform migrations from Tableau Server to Tableau Cloud.
    /// </summary>
    public class ServerToCloudMigrationPipeline : MigrationPipelineBase
    {
        /// <summary>
        /// Content types that are supported for migrations.
        /// </summary>
        public static readonly ImmutableArray<MigrationPipelineContentType> ContentTypes =
            new MigrationPipelineContentType[]
            {
                new MigrationPipelineContentType<IUser>(),
                new MigrationPipelineContentType<IGroup, IPublishableGroup>(),
                new MigrationPipelineContentType<IProject>(),
                new MigrationPipelineContentType<IDataSource, IPublishableDataSource>(),
                new MigrationPipelineContentType<IWorkbook, IPublishableWorkbook, IResultWorkbook>(),
            }.ToImmutableArray();

        /// <summary>
        /// Creates a new <see cref="ServerToCloudMigrationPipeline"/> object.
        /// </summary>
        /// <param name="services"><inheritdoc /></param>
        public ServerToCloudMigrationPipeline(IServiceProvider services)
            : base(services)
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
        }

        /// <inheritdoc />
        public override IContentBatchMigrator<TContent> GetBatchMigrator<TContent>()
        {
            switch (typeof(TContent))
            {
                case Type user when user == typeof(IUser):
                    return Services.GetRequiredService<BulkPublishContentBatchMigrator<TContent>>();
                case Type group when group == typeof(IGroup):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent, IPublishableGroup>>();
                case Type project when project == typeof(IProject):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();
                case Type dataSource when dataSource == typeof(IDataSource):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent, IPublishableDataSource>>();
                case Type worbook when worbook == typeof(IWorkbook):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent, IPublishableWorkbook, IResultWorkbook>>();
                default:
                    return base.GetBatchMigrator<TContent>();
            }
        }
    }
}
