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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks
{
    internal class SubscriptionsCapabilityManager : MigrationCapabilityManagerBase, ISubscriptionsCapabilityManager
    {
        private readonly IMigration _migration;
        private readonly IDestinationEndpoint _destinationEndpoint;

        public SubscriptionsCapabilityManager(
            IDestinationEndpoint destinationEndpoint,
            IMigrationCapabilitiesEditor capabilities,
            ISharedResourcesLocalizer localizer,
            ILogger<SubscriptionsCapabilityManager> logger,
            IMigration migration) : base(localizer, logger, capabilities)
        {
            _destinationEndpoint = destinationEndpoint;
            _migration = migration;
        }

        /// <inheritdoc/>
        public override Task<IResult> SetMigrationCapabilityAsync(IServerSession destinationServerSession, CancellationToken cancel)
        {
            if (destinationServerSession.Settings?.DisableSubscriptions == true)
            {
                Logger.LogInformation(Localizer[SharedResourceKeys.SubscriptionsDisabledReason]);
                CapabilitiesEditor.ContentTypesDisabledAtDestination.Add(typeof(ISubscription<>));
            }

            return Task.FromResult<IResult>(Result.Succeeded());
        }
    }
}
