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
using System.ComponentModel.DataAnnotations;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Default <see cref="IMigrationPlan"/> implementation.
    /// </summary>
    /// <param name="PlanId"><inheritdoc /></param>
    /// <param name="PipelineProfile"><inheritdoc /></param>
    /// <param name="Options"><inheritdoc /></param>
    /// <param name="Source"><inheritdoc /></param>
    /// <param name="Destination"><inheritdoc /></param>
    /// <param name="Hooks"><inheritdoc /></param>
    /// <param name="Mappings"><inheritdoc /></param>
    /// <param name="Filters"><inheritdoc /></param>
    /// <param name="Transformers"><inheritdoc /></param>
    /// <param name="PipelineFactoryOverride"><inheritdoc /></param>
    public record MigrationPlan(
        Guid PlanId,
        [property: EnumDataType(typeof(PipelineProfile))] PipelineProfile PipelineProfile,
        IMigrationPlanOptionsCollection Options,
        IMigrationPlanEndpointConfiguration Source,
        IMigrationPlanEndpointConfiguration Destination,
        IMigrationHookFactoryCollection Hooks,
        IMigrationHookFactoryCollection Mappings,
        IMigrationHookFactoryCollection Filters,
        IMigrationHookFactoryCollection Transformers,
        Func<IServiceProvider, IMigrationPipelineFactory>? PipelineFactoryOverride
    ) : IMigrationPlan
    { }
}
