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
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Default <see cref="IMigrationPipelineFactory"/> implementation.
    /// </summary>
    public class MigrationPipelineFactory : IMigrationPipelineFactory
    {
        /// <summary>
        /// Gets the migration-scoped service provider.
        /// </summary>
        protected IServiceProvider Services { get; }

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineFactory"/> object.
        /// </summary>
        /// <param name="services">A service provider to create pipelines with.</param>
        public MigrationPipelineFactory(IServiceProvider services)
        {
            Services = services;
        }

        /// <inheritdoc />
        public virtual IMigrationPipeline Create(IMigrationPlan plan)
        {
            switch (plan.PipelineProfile)
            {
                case PipelineProfile.ServerToCloud:
                    return Services.GetRequiredService<ServerToCloudMigrationPipeline>();

                case PipelineProfile.ServerToServer:
                    return Services.GetRequiredService<ServerToServerMigrationPipeline>();

                case PipelineProfile.CloudToCloud:
                    return Services.GetRequiredService<CloudToCloudMigrationPipeline>();

                default:
                    throw new ArgumentException($"Cannot create a migration pipeline for profile {plan.PipelineProfile}");
            }
        }
    }
}
