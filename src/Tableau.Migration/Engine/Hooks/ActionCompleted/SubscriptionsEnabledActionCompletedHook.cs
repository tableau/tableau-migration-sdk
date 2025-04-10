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
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Hooks.ActionCompleted
{
    /// <summary>
    /// This hook checks if subscriptions are enabled in case the check did not already run.
    /// </summary>
    public class SubscriptionsEnabledActionCompletedHook : IMigrationActionCompletedHook
    {
        private IMigrationPipelineRunner _pipelineRunner;
        private readonly ISubscriptionsCapabilityManager _subscriptionsCapabilityManager;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="pipelineRunner">The pipeline runner.</param>
        /// <param name="subscriptionsCapabilityManager">The subscriptions capability manager.</param>
        public SubscriptionsEnabledActionCompletedHook(
            IMigrationPipelineRunner pipelineRunner,
            ISubscriptionsCapabilityManager subscriptionsCapabilityManager)
        {
            _pipelineRunner = pipelineRunner;
            _subscriptionsCapabilityManager = subscriptionsCapabilityManager;
        }

        /// <inheritdoc/>
        public async Task<IMigrationActionResult?> ExecuteAsync(IMigrationActionResult ctx, CancellationToken cancel)
        {
            var action = _pipelineRunner.CurrentAction;
            if (action is not MigrateContentAction<IWorkbook>)
            {
                return ctx;
            }

            if (_subscriptionsCapabilityManager.IsMigrationCapabilityDisabled())
            {
                return ctx;
            }

            var result = await _subscriptionsCapabilityManager.SetMigrationCapabilityAsync(cancel).ConfigureAwait(false);

            if (result.Success)
            {
                return ctx;
            }

            return MigrationActionResult.Failed(result.Errors);
        }
    }
}
