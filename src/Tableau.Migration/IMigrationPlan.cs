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
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that describes how to perform a migration of Tableau data between sites.
    /// </summary>
    public interface IMigrationPlan
    {
        /// <summary>
        /// Gets a unique identifier for the plan, generated at time of creation.
        /// </summary>
        Guid PlanId { get; }

        /// <summary>
        /// Gets the profile of the pipeline that will be built and executed.
        /// </summary>
        PipelineProfile PipelineProfile { get; }

        /// <summary>
        /// Gets the per-plan options.
        /// </summary>
        IMigrationPlanOptionsCollection Options { get; }

        /// <summary>
        /// Gets the defined source endpoint configuration.
        /// </summary>
        IMigrationPlanEndpointConfiguration Source { get; }

        /// <summary>
        /// Gets the defined destination endpoint configuration.
        /// </summary>
        IMigrationPlanEndpointConfiguration Destination { get; }

        /// <summary>
        /// Gets the collection of registered hooks for each hook type.
        /// </summary>
        IMigrationHookFactoryCollection Hooks { get; }

        /// <summary>
        /// Gets the collection of registered mappings for each content type.
        /// </summary>
        IMigrationHookFactoryCollection Mappings { get; }

        /// <summary>
        /// Gets the collection of registered filters for each content type.
        /// </summary>
        IMigrationHookFactoryCollection Filters { get; }

        /// <summary>
        /// Gets the collection of registered transformers for each content type.
        /// </summary>
        IMigrationHookFactoryCollection Transformers { get; }

        /// <summary>
        /// Gets the pipeline factory to use to create the pipeline during migration.
        /// </summary>
        Func<IServiceProvider, IMigrationPipelineFactory>? PipelineFactoryOverride { get; }
    }
}
