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

using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.InitializeMigration.Default
{
    /// <summary>
    /// <see cref="IInitializeMigrationHook"/> that populates the <see cref="IMigrationCapabilities"/> 
    /// by running all <see cref="IMigrationCapabilityManager"/>s.
    /// </summary>
    public class InitializeCapabilitiesHook : IInitializeMigrationHook
    {
        private static readonly PropertyInfo[] _capabilitiesProperties = typeof(IMigrationCapabilities).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        private readonly IMigrationCapabilitiesEditor _capabilities;
        private readonly IEnumerable<IMigrationCapabilityManager> _capabilityManagers;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<InitializeCapabilitiesHook> _logger;

        /// <summary>
        /// Creates a new <see cref="InitializeCapabilitiesHook"/> object.
        /// </summary>
        /// <param name="capabilities">The migration capabilities</param>
        /// <param name="capabilityManagers">The DI registered capability managers to run.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="logger">The logger.</param>
        public InitializeCapabilitiesHook(IMigrationCapabilitiesEditor capabilities,
            IEnumerable<IMigrationCapabilityManager> capabilityManagers,
            ISharedResourcesLocalizer localizer,
            ILogger<InitializeCapabilitiesHook> logger)
        {
            _capabilities = capabilities;
            _capabilityManagers = capabilityManagers;
            _localizer = localizer;
            _logger = logger;
        }

        /// <summary>
        /// Logs the differences between the original and current capabilities.
        /// </summary>
        /// <param name="original"></param>
        private void LogChangedCapabilities(IMigrationCapabilities original)
        {
            // No logger passed in, skip all this
            bool anyChanges = false;

            foreach (var property in _capabilitiesProperties)
            {
                var originalValue = property.GetValue(original);
                var currentValue = property.GetValue(_capabilities);

                if (!Equals(originalValue, currentValue))
                {
                    _logger.LogDebug(
                        _localizer[SharedResourceKeys.InitializeMigrationBaseDebugMessage],
                        nameof(InitializeCapabilitiesHook),
                        property.Name,
                        originalValue,
                        currentValue);
                    anyChanges = true;
                }
            }

            if (!anyChanges)
            {
                _logger.LogDebug(_localizer[SharedResourceKeys.InitializeMigrationBaseNoChangesMessage], nameof(InitializeCapabilitiesHook));
            }
        }

        /// <inheritdoc />
        public async Task<IInitializeMigrationHookResult?> ExecuteAsync(IInitializeMigrationHookResult ctx, CancellationToken cancel)
        {
            // Deep copy the capabilities.
            IMigrationCapabilities capabilitiesOriginal = _capabilities.Clone();

            var results = new ResultBuilder();
            foreach(var capabilityManager in _capabilityManagers)
            {
                var managerResult = await capabilityManager.SetMigrationCapabilityAsync(ctx, cancel)
                    .ConfigureAwait(false);

                results.Add(managerResult);
            }

            // Log changes.
            LogChangedCapabilities(capabilitiesOriginal);

            var result = results.Build();
            if(!result.Success)
            {
                return ctx.ToFailure(result.Errors);
            }

            return ctx;
        }
    }
}
