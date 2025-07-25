﻿
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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities
{
    /// <summary>
    /// Manages capabilities related to content types or post publish operations.
    /// </summary>
    public interface IMigrationCapabilityManager
    {
        /// <summary>
        /// Sets the migration capability.
        /// </summary>
        /// <param name="ctx">The preflight context.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of setting the capability. Return a failure result to fail the preflight check and prevent migration from continuing.</returns>
        Task<IResult> SetMigrationCapabilityAsync(IInitializeMigrationContext ctx, CancellationToken cancel);
    }
}
