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
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Interface for an object representing an in-progress migration. 
    /// This interface or its properties can be obtained through scoped dependency injection.
    /// </summary>
    public interface IMigration
    {
        /// <summary>
        /// Gets the unique ID of the migration run, generated each time a migration run starts.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the migration source endpoint to pull Tableau data from.
        /// </summary>
        ISourceEndpoint Source { get; }

        /// <summary>
        /// Gets the migration destination endpoint to push Tableau data to.
        /// </summary>
        IDestinationEndpoint Destination { get; }

        /// <summary>
        /// Gets the migration plan being run.
        /// </summary>
        IMigrationPlan Plan { get; }

        /// <summary>
        /// Gets the current migration manifest.
        /// </summary>
        IMigrationManifestEditor Manifest { get; }

        /// <summary>
        /// Gets the migration pipeline being executed.
        /// </summary>
        IMigrationPipeline Pipeline { get; }
    }
}
