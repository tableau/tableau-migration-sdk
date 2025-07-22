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
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Actions;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Abstract base class for <see cref="IMigrationPipeline"/> implementations that migration Tableau content.
    /// </summary>
    public abstract class TableauMigrationPipelineBase : MigrationPipelineBase
    {
        /// <summary>
        /// Creates a new <see cref="TableauMigrationPipelineBase"/>
        /// </summary>
        /// <param name="services"><inheritdoc /></param>
        /// <param name="configReader"><inheritdoc /></param>
        protected TableauMigrationPipelineBase(IServiceProvider services, IConfigReader configReader)
            : base(services, configReader)
        { }

        /// <summary>
        /// Creates the migration action for handling the extract refresh task content type.
        /// </summary>
        /// <returns>The created migration action.</returns>
        protected abstract IMigrationAction CreateExtractRefreshTaskAction();

        /// <summary>
        /// Creates the migration action for handling the subscription content type.
        /// </summary>
        /// <returns>The created migration action.</returns>
        protected abstract IMigrationAction CreateSubscriptionAction();

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
            yield return CreateMigrateContentAction<IGroupSet>();
            yield return CreateMigrateContentAction<IProject>();
            yield return CreateMigrateContentAction<IDataSource>();
            yield return CreateMigrateContentAction<IWorkbook>();
            yield return CreateExtractRefreshTaskAction();
            yield return CreateMigrateContentAction<ICustomView>();
            yield return CreateSubscriptionAction();
            yield return CreateMigrateContentAction<IFavorite>();
            
        }
    }
}
