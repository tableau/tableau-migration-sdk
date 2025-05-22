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
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.InitializeMigration.Default
{
    internal class SubscriptionsPreflightCheck : InitializeMigrationCapabilityHookBase
    {
        private readonly ISubscriptionsCapabilityManager _capabilityManager;

        public SubscriptionsPreflightCheck(
           ISharedResourcesLocalizer? localizer,
           ILogger<SubscriptionsPreflightCheck>? logger,
           IMigrationCapabilitiesEditor capabilities,
           ISubscriptionsCapabilityManager capabilityManager) : base(localizer, logger, capabilities)
        {
            _capabilityManager = capabilityManager;
        }

        public override async Task<IInitializeMigrationHookResult?> ExecuteCheckAsync(IInitializeMigrationHookResult ctx, CancellationToken cancel)
        {
            var capabilitySetResult = await _capabilityManager.SetMigrationCapabilityAsync(ctx.DestinationServerSession, cancel).ConfigureAwait(false);

            if (!capabilitySetResult.Success)
            {
                return ctx.ToFailure(capabilitySetResult.Errors);
            }

            return ctx;
        }
    }
}