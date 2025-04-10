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

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.InitializeMigration
{
    internal abstract class InitializeMigrationCapabilityHookBase : IInitializeMigrationHook
    {
        private readonly string _typeName;
        private readonly PropertyInfo[] _capabilitiesProperties;

        protected readonly ISharedResourcesLocalizer? Localizer;
        protected readonly ILogger<IInitializeMigrationHook>? Logger;

        protected readonly IMigrationCapabilitiesEditor Capabilities;

        public InitializeMigrationCapabilityHookBase(
            ISharedResourcesLocalizer? localizer,
            ILogger<IInitializeMigrationHook>? logger,
            IMigrationCapabilitiesEditor capabilities)
        {
            Localizer = localizer;
            Logger = logger;
            Capabilities = capabilities;

            _typeName = GetType().GetFormattedName();
            _capabilitiesProperties = typeof(MigrationCapabilities).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <inheritdoc />
        public async Task<IInitializeMigrationHookResult?> ExecuteAsync(IInitializeMigrationHookResult ctx, CancellationToken cancel)
        {
            // Deep copy the Capabilities
            IMigrationCapabilities capabilitiesOriginal = Capabilities.Clone();

            var ret = await ExecuteCheckAsync(ctx, cancel).ConfigureAwait(false);

            // Log changes
            LogChangedCapabilities(capabilitiesOriginal);

            return ret;
        }

        /// <summary>
        /// Logs the differences between the original and current capabilities.
        /// </summary>
        /// <param name="original"></param>
        private void LogChangedCapabilities(IMigrationCapabilities original)
        {
            // No logger passed in, skip all this
            if (Logger is null || Localizer is null)
            {
                return;
            }

            bool anyChanges = false;

            foreach (var property in _capabilitiesProperties)
            {
                var originalValue = property.GetValue(original);
                var currentValue = property.GetValue(Capabilities);

                if (!Equals(originalValue, currentValue))
                {
                    Logger.LogDebug(
                        Localizer[SharedResourceKeys.InitializeMigrationBaseDebugMessage],
                        _typeName,
                        property.Name,
                        originalValue,
                        currentValue);
                    anyChanges = true;
                }
            }

            if (!anyChanges)
            {
                Logger.LogDebug(Localizer[SharedResourceKeys.InitializeMigrationBaseNoChangesMessage], _typeName);
            }
        }

        /// <summary>
        /// Executes a check for capabilities.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous hook.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>
        /// A task to await containing the context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        public abstract Task<IInitializeMigrationHookResult?> ExecuteCheckAsync(IInitializeMigrationHookResult ctx, CancellationToken cancel);
    }
}
