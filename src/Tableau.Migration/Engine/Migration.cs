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
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Default <see cref="IMigration"/> implementation.
    /// </summary>
    public class Migration : IMigration
    {
        /// <summary>
        /// Creates a new <see cref="Migration"/> object.
        /// </summary>
        /// <param name="services">The service provider to use to initialize the migration.</param>
        public Migration(IServiceProvider services)
        {
            var input = services.GetRequiredService<IMigrationInput>();
            Id = input.MigrationId;
            Plan = input.Plan;

            var pipelineFactory = Plan.PipelineFactoryOverride?.Invoke(services) ?? services.GetRequiredService<IMigrationPipelineFactory>();
            Pipeline = pipelineFactory.Create(Plan);

            var endpointFactory = services.GetRequiredService<IMigrationEndpointFactory>();
            Source = endpointFactory.CreateSource(Plan);
            Destination = endpointFactory.CreateDestination(Plan);

            var manifestFactory = services.GetRequiredService<IMigrationManifestFactory>();
            Manifest = manifestFactory.Create(input, Id);
        }

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public ISourceEndpoint Source { get; }

        /// <inheritdoc />
        public IDestinationEndpoint Destination { get; }

        /// <inheritdoc />
        public IMigrationPlan Plan { get; }

        /// <inheritdoc />
        public IMigrationManifestEditor Manifest { get; }

        /// <inheritdoc />
        public IMigrationPipeline Pipeline { get; }
    }
}
