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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that can migration Tableau data between Tableau sites.
    /// </summary>
    public interface IMigrator
    {
        /// <summary>
        /// Executes a migration asynchronously.
        /// </summary>
        /// <param name="plan">The migration plan to execute.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The results of the migration.</returns>
        Task<MigrationResult> ExecuteAsync(IMigrationPlan plan, CancellationToken cancel);

        /// <summary>
        /// Executes a migration asynchronously.
        /// </summary>
        /// <param name="plan">The migration plan to execute.</param>
        /// <param name="previousManifest">A manifest from a previous migration of the same plan to use to determine what progress has already been made.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The results of the migration.</returns>
        Task<MigrationResult> ExecuteAsync(IMigrationPlan plan, IMigrationManifest? previousManifest, CancellationToken cancel);
    }
}
