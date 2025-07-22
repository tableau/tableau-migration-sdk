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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.TestApplication.Hooks.ActionCompleted
{
    /// <summary>
    /// This hook logs when an action was completed. Ideally I would know this action, but that's not possible right now
    /// </summary>
    internal class TimeLoggerAfterActionHook : IMigrationActionCompletedHook
    {
        private ILogger<TimeLoggerAfterActionHook> _logger;
        private MigrationPipelineRunner _pipelineRunner;

        public TimeLoggerAfterActionHook(
            ILogger<TimeLoggerAfterActionHook> logger,
            IMigrationPipelineRunner pipelineRunner)
        {
            _logger = logger;
            _pipelineRunner = (MigrationPipelineRunner)pipelineRunner;
        }

        public Task<IMigrationActionResult?> ExecuteAsync(IMigrationActionResult ctx, CancellationToken cancel)
        {
            string actionName = _pipelineRunner.CurrentAction?.GetType().GetFormattedName() ?? "Unknown";

            _logger.LogInformation($"Action {actionName} completed");
            return Task.FromResult((IMigrationActionResult?)ctx);
        }
    }
}
