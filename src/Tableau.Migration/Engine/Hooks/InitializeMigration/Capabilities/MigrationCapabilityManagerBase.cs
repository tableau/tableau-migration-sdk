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

namespace Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities
{
    /// <summary>
    /// Standard base class for <see cref="IMigrationCapabilityManager"/> implementations.
    /// </summary>
    public abstract class MigrationCapabilityManagerBase : IMigrationCapabilityManager
    {
        /// <summary>
        /// Gets the migration capabilities to populate/manage.
        /// </summary>
        protected IMigrationCapabilitiesEditor Capabilities { get; }

        /// <summary>
        /// Gets the localizer.
        /// </summary>
        protected ISharedResourcesLocalizer Localizer { get;  }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Creates a new <see cref="MigrationCapabilityManagerBase"/> object.
        /// </summary>
        /// <param name="capabilities">The migration capabilities to populate/manage.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="logger">The logger.</param>
        public MigrationCapabilityManagerBase(IMigrationCapabilitiesEditor capabilities,
            ISharedResourcesLocalizer localizer, ILogger logger)
        {
            Capabilities = capabilities;
            Localizer = localizer;
            Logger = logger;
        }

        /// <inheritdoc/>
        public abstract Task<IResult> SetMigrationCapabilityAsync(IInitializeMigrationContext ctx, CancellationToken cancel);

        /// <summary>
        /// Logs a warning that a content type will be disabled for a capability reason.
        /// </summary>
        /// <param name="typeName">The content type name.</param>
        /// <param name="reason">The reason for the capability being disabled.</param>
        protected void LogCapabilityDisabled(string typeName, string reason)
            => Logger.LogWarning(Localizer[SharedResourceKeys.MigrationDisabledWarning], typeName, reason);
    }
}