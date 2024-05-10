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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// <see cref="IMigrationAction"/> implementation that validates that the migration is ready to begin.
    /// </summary>
    public class PreflightAction : IMigrationAction
    {
        private readonly PreflightOptions _options;
        private readonly IMigration _migration;
        private readonly ILogger<PreflightAction> _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="PreflightAction"/> object.
        /// </summary>
        /// <param name="options">The preflight options.</param>
        /// <param name="migration">The current migration.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="localizer">A localizer.</param>
        public PreflightAction(IOptions<PreflightOptions> options, IMigration migration, 
            ILogger<PreflightAction> logger, ISharedResourcesLocalizer localizer)
        {
            _options = options.Value;
            _migration = migration;
            _logger = logger;
            _localizer = localizer;
        }

        private void ValidateExtractEncryptionSetting(ISiteSettings source, ISiteSettings destination)
        {
            /* Any destination value is valid if source has encryption disabled.
             * There won't be extract migration errors and there may be destination extracts encrypted.
             */
            if (ExtractEncryptionModes.IsAMatch(source.ExtractEncryptionMode, ExtractEncryptionModes.Disabled))
            {
                return;
            }

            /* We don't care if the destination site is enforced/enabled as long as it supports encrypted extracts.
             * There won't be extract migration errors and the destination gets to keep its preference.
             */
            if (!ExtractEncryptionModes.IsAMatch(destination.ExtractEncryptionMode, ExtractEncryptionModes.Disabled))
            {
                return;
            }

            // Warn the user about the potential failure of encrypted extract migration.
            _logger.LogWarning(_localizer[SharedResourceKeys.SiteSettingsExtractEncryptionDisabledLogMessage]);
        }

        private async ValueTask<IResult> ManageSettingsAsync(CancellationToken cancel)
        {
            if(!_options.ValidateSettings)
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

            if(!sourceSessionResult.Success || !destinationSessionResult.Success)
            {
                return new ResultBuilder().Add(sourceSessionResult, destinationSessionResult).Build();
            }

            // Find if we have access to validate settings.
            var sourceSession = sourceSessionResult.Value;
            var destinationSession = destinationSessionResult.Value;

            if(!sourceSession.IsAdministrator || !destinationSession.IsAdministrator)
            {
                _logger.LogDebug(_localizer[SharedResourceKeys.SiteSettingsSkippedNoAccessLogMessage]);
                return Result.Succeeded();
            }

            // Validate supported settings.

            ValidateExtractEncryptionSetting(sourceSession.Settings, destinationSession.Settings);            

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

            var settingsResult = await ManageSettingsAsync(cancel).ConfigureAwait(false);

            return MigrationActionResult.FromResult(settingsResult);
        }
    }
}
