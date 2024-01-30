// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// <see cref="IMigrationAction"/> implementation that validates that the migration is ready to begin.
    /// </summary>
    public class PreflightAction : IMigrationAction
    {
        /// <inheritdoc />
        public Task<IMigrationActionResult> ExecuteAsync(CancellationToken cancel)
        {
            //TODO (W-12586258): Preflight action should validate that hook factories return the right type.
            //TODO (W-12586258): Preflight action should validate endpoints beyond simple initialization.
            return Task.FromResult((IMigrationActionResult)MigrationActionResult.Succeeded());
        }
    }
}
