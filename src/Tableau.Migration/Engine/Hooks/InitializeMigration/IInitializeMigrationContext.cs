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

namespace Tableau.Migration.Engine.Hooks.InitializeMigration
{
    /// <summary>
    /// Interface for a read-only context used in <see cref="IInitializeMigrationHook"/>s.
    /// </summary>
    public interface IInitializeMigrationContext
    {
        /// <summary>
        /// Gets the migration-scoped service provider.
        /// </summary>
        IServiceProvider ScopedServices { get; }

        /// <summary>
        /// Gets the source endpoint's preflight context information.
        /// </summary>
        IEndpointPreflightContext Source { get; }

        /// <summary>
        /// Gets the destination endpoint's preflight context information.
        /// </summary>
        IEndpointPreflightContext Destination { get; }
    }
}
