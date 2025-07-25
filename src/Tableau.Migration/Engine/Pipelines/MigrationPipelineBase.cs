﻿//
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
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Conversion;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Preparation;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Common base class for <see cref="IMigrationPipeline"/> implementations that can create and execute actions.
    /// </summary>
    public abstract class MigrationPipelineBase : IMigrationPipeline
    {
        /// <summary>
        /// Gets the service provider.
        /// </summary>
        protected IServiceProvider Services { get; }

        /// <summary>
        /// Gets the config reader.
        /// </summary>
        protected readonly IConfigReader ConfigReader;

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineBase"/> object.
        /// </summary>
        /// <param name="services">A DI service provider to create actions with.</param>
        /// <param name="configReader">The config reader.</param>
        protected MigrationPipelineBase(
            IServiceProvider services,
            IConfigReader configReader)
        {
            Services = services;
            ConfigReader = configReader;
        }

        #region - Protected Methods -

        /// <summary>
        /// Creates an action of the given type.
        /// </summary>
        /// <typeparam name="TAction">The action type.</typeparam>
        /// <returns>The created action.</returns>
        protected TAction CreateAction<TAction>()
            where TAction : IMigrationAction
            => Services.GetRequiredService<TAction>();

        /// <summary>
        /// Creates an action to migrate content of a given type.
        /// </summary>
        /// <typeparam name="TContent">The content type to migrate.</typeparam>
        /// <returns>The created content migration action.</returns>
        protected IMigrateContentAction<TContent> CreateMigrateContentAction<TContent>()
            where TContent : class, IContentReference
            => CreateAction<MigrateContentAction<TContent>>();

        /// <summary>
        /// Builds the pipeline of actions to execute in order.
        /// </summary>
        /// <returns>The actions to execute in order.</returns>
        protected abstract IEnumerable<IMigrationAction> BuildPipeline();

        #endregion

        #region - IMigrationPipeline Implementation -

        /// <inheritdoc />
        public ImmutableArray<IMigrationAction> BuildActions() => BuildPipeline().ToImmutableArray();

        /// <inheritdoc />
        public virtual IContentMigrator<TContent> GetMigrator<TContent>()
            where TContent : class, IContentReference
        {
            return Services.GetRequiredService<ContentMigrator<TContent>>();
        }

        /// <inheritdoc />
        public virtual IContentBatchMigrator<TContent> GetBatchMigrator<TContent>()
            where TContent : class, IContentReference
        {
            switch (typeof(TContent))
            {
                case Type user when user == typeof(IUser):
                    if (ConfigReader.Get<IUser>().BatchPublishingEnabled)
                    {
                        return Services.GetRequiredService<BulkPublishContentBatchMigrator<TContent>>();
                    }
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();

                case Type group when group == typeof(IGroup):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent, IPublishableGroup>>();

                case Type groupSet when groupSet == typeof(IGroupSet):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent, IPublishableGroupSet, IPublishableGroupSet>>();

                case Type project when project == typeof(IProject):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();

                case Type dataSource when dataSource == typeof(IDataSource):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent, IPublishableDataSource, IDataSourceDetails>>();

                case Type workbook when workbook == typeof(IWorkbook):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent, IPublishableWorkbook, IWorkbookDetails>>();

                case Type customView when customView == typeof(ICustomView):
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent, IPublishableCustomView, ICustomView>>();

                default:
                    return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();
            }
        }

        /// <inheritdoc />
        public virtual IContentItemPreparer<TContent, TPublish> GetItemPreparer<TContent, TPrepare, TPublish>()
            where TContent : class
            where TPrepare : class
            where TPublish : class
        {
            switch (typeof(TContent))
            {
                case Type source when source == typeof(TPrepare) && source == typeof(TPublish):
                    return (IContentItemPreparer<TContent, TPublish>)Services.GetRequiredService<SourceContentItemPreparer<TContent>>();
                case Type source when source == typeof(TContent) && source == typeof(TPrepare):
                    return Services.GetRequiredService<SourceContentItemPreparer<TContent, TPublish>>();
                case Type source when source == typeof(IServerExtractRefreshTask) && typeof(TPublish) == typeof(ICloudExtractRefreshTask):
                    return (IContentItemPreparer<TContent, TPublish>)Services.GetRequiredService<ExtractRefreshTaskServerToCloudPreparer>();
                default:
                    return Services.GetRequiredService<EndpointContentItemPreparer<TContent, TPrepare, TPublish>>();
            }
        }

        /// <inheritdoc />
        public virtual IContentItemConverter<TPrepare, TPublish> GetItemConverter<TPrepare, TPublish>()
            where TPrepare : class
            where TPublish : class
            => Services.GetRequiredService<DirectContentItemConverter<TPrepare, TPublish>>();

        /// <inheritdoc />
        public virtual IContentReferenceCache CreateSourceCache<TContent>()
            where TContent : class, IContentReference
            => Services.GetRequiredService<BulkSourceCache<TContent>>();

        /// <inheritdoc />
        public virtual IContentReferenceCache CreateDestinationCache<TContent>()
            where TContent : class, IContentReference
        {
            switch (typeof(TContent))
            {
                case Type project when project == typeof(IProject):
                    return Services.GetRequiredService<BulkDestinationProjectCache>();
                default:
                    return Services.GetRequiredService<BulkDestinationCache<TContent>>();
            }
        }

        /// <inheritdoc />
        public virtual ILockedProjectCache GetDestinationLockedProjectCache()
            => Services.GetRequiredService<BulkDestinationProjectCache>();

        #endregion
    }
}
