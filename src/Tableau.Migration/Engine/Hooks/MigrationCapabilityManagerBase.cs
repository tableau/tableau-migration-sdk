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
using Tableau.Migration.Content;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks
{
    internal abstract class MigrationCapabilityManagerBase : IMigrationCapabilityManager
    {
        protected readonly ISharedResourcesLocalizer Localizer;
        protected readonly ILogger Logger;
        protected readonly IMigrationCapabilitiesEditor CapabilitiesEditor;

        public MigrationCapabilityManagerBase(
            ISharedResourcesLocalizer localizer,
            ILogger logger,
            IMigrationCapabilitiesEditor capabilitiesEditor)
        {
            Localizer = localizer;
            Logger = logger;
            CapabilitiesEditor = capabilitiesEditor;
        }

        /// <inheritdoc/>
        public abstract Task<IResult> SetMigrationCapabilityAsync(IServerSession destinationServerSession, CancellationToken cancel);

        protected void LogCapabilityDisabled(string typeName, string reason)
            => Logger.LogWarning(Localizer[SharedResourceKeys.MigrationDisabledWarning], typeName, reason);
    }
}