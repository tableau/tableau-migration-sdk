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
using Microsoft.Extensions.Options;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// <see cref="IMigrationAction"/> implementation that validates that the migration is ready to begin.
    /// </summary>
    public class PreflightAction : IMigrationAction
    {
        private readonly IServiceProvider _services;
        private readonly PreflightOptions _options;
        private readonly IMigrationHookRunner _hooks;
        private readonly IMigration _migration;
        private readonly ILogger<PreflightAction> _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="PreflightAction"/> object.
        /// </summary>
        /// <param name="services">The migration-scoped service provider.</param>
        /// <param name="options">The preflight options.</param>
        /// <param name="migration">The current migration.</param>
        /// <param name="hooks">The hook runner</param>
        /// <param name="logger">A logger.</param>
        /// <param name="localizer">A localizer.</param>
        public PreflightAction(IServiceProvider services, IOptions<PreflightOptions> options,
           IMigration migration, IMigrationHookRunner hooks,
           ILogger<PreflightAction> logger, ISharedResourcesLocalizer localizer)
        {
            _services = services;
            _options = options.Value;
            _migration = migration;
            _hooks = hooks;
            _logger = logger;
            _localizer = localizer;
        }

        private async ValueTask<IResult> ManageSettingsAsync(CancellationToken cancel)
        {
            if (!_options.ValidateSettings)
            {
                _logger.LogDebug(_localizer[SharedResourceKeys.SiteSettingsSkippedDisabledLogMessage]);
                return Result.Succeeded();
            }

            // Get the source and destination settings to compare concurrently.
            var sourceSessionTask = _migration.Source.GetSessionAsync(cancel);
            var destinationSessionTask = _migration.Destination.GetSessionAsync(cancel);

            await Task.WhenAll(sourceSessionTask, destinationSessionTask).ConfigureAwait(false);

            var sourceSessionResult = sourceSessionTask.Result;
            var destinationSessionResult = destinationSessionTask.Result;

            if (!sourceSessionResult.Success || !destinationSessionResult.Success)
            {
                return new ResultBuilder().Add(sourceSessionResult, destinationSessionResult).Build();
            }

            // Find if we have access to validate settings.
            var sourceSession = sourceSessionResult.Value;
            var destinationSession = destinationSessionResult.Value;

            if (!sourceSession.IsAdministrator || !destinationSession.IsAdministrator)
            {
                _logger.LogDebug(_localizer[SharedResourceKeys.SiteSettingsSkippedNoAccessLogMessage]);
                return Result.Succeeded();
            }

            /* We currently don't update settings for the user because
             * Tableau Cloud returns an error when site administrators update site settings,
             * requiring server administrator access that Tableau Cloud users cannot have.
             * 
             * If/when that gets addressed we can update the destination setting automatically.
             */

            return Result.Succeeded();
        }

        /// <inheritdoc />
        public async Task<IMigrationActionResult> ExecuteAsync(CancellationToken cancel)
        {
            //TODO (W-12586258): Preflight action should validate that hook factories return the right type.
            //TODO (W-12586258): Preflight action should validate endpoints beyond simple initialization.

            var preflightResultBuilder = new ResultBuilder();

            var settingsResult = await ManageSettingsAsync(cancel).ConfigureAwait(false);

            if (!settingsResult.Success)
            {
                return MigrationActionResult.FromResult(settingsResult);
            }

            // Call user-registered initializer last so the hook can rely that all engine initialization/validation is complete.
            IInitializeMigrationHookResult initResult = InitializeMigrationHookResult.Succeeded(_services);

            initResult = await _hooks.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(initResult, cancel).ConfigureAwait(false);

            preflightResultBuilder.Add(settingsResult, initResult);
            return MigrationActionResult.FromResult(preflightResultBuilder.Build());
        }
    }
}
