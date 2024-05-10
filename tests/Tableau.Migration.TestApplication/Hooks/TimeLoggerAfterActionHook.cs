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

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;

namespace Tableau.Migration.TestApplication.Hooks
{
    /// <summary>
    /// This hook logs when an action was completed. Ideally I would know this action, but that's not possible right now
    /// </summary>
    internal class TimeLoggerAfterActionHook : IMigrationActionCompletedHook
    {
        private ILogger<TimeLoggerAfterActionHook> _logger;

        public TimeLoggerAfterActionHook(ILogger<TimeLoggerAfterActionHook> logger )
        {
            _logger = logger;
        }

        public Task<IMigrationActionResult?> ExecuteAsync(IMigrationActionResult ctx, CancellationToken cancel)
        {
            _logger.LogInformation("Migration action completed");
            return Task.FromResult((IMigrationActionResult?)ctx);
        }
    }
}
