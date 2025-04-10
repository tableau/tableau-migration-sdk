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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks
{
    internal class EmbeddedCredentialsCapabilityManager : MigrationCapabilityManagerBase, IEmbeddedCredentialsCapabilityManager
    {
        private readonly IMigration _migration;
        private readonly ITableauApiEndpointConfiguration? _destinationConfig; //Null if the destination is not an API.

        public EmbeddedCredentialsCapabilityManager(
            ISharedResourcesLocalizer localizer,
            ILogger<EmbeddedCredentialsCapabilityManager> logger,
            IMigrationCapabilitiesEditor capabilitiesEditor,
            IMigration migration) : base(localizer, logger, capabilitiesEditor)
        {
            _migration = migration;
            _destinationConfig = migration.Plan.Destination as ITableauApiEndpointConfiguration;
        }

        /// <inheritdoc/>
        public override bool IsMigrationCapabilityDisabled() => CapabilitiesEditor.EmbeddedCredentialsDisabled;

        /// <inheritdoc/>
        public override async Task<IResult> SetMigrationCapabilityAsync(CancellationToken cancel)
        {
            // Get the destination info which needs to be passed to the source for retrieving the keychain.
            var destinationSiteInfoResult = await GetDestinationSiteInfoAsync(cancel).ConfigureAwait(false);

            if (!destinationSiteInfoResult.Success)
            {
                return Result.Failed(destinationSiteInfoResult.Errors);
            }

            // Retrieve a fake keychain so we can check the error code
            var retrieveKeyChainResult = await RetrieveKeychainAsync(destinationSiteInfoResult.Value, cancel).ConfigureAwait(false);

            // No error code means embedded creds work. This should never happen as we're getting a fake keychain.
            if (retrieveKeyChainResult.Success)
            {
                CapabilitiesEditor.EmbeddedCredentialsDisabled = false;
                return Result.Succeeded();
            }

            // Check the error codes for the embedded credential migration disabled code.
            //
            // At this point, the retrieveKeychainResult should have failed with the error code,
            // either because embedded creds are not supported, or if it is supported, then the fake keychain shouldn't exist
            CapabilitiesEditor.EmbeddedCredentialsDisabled = IsEmbeddedCredentialMigrationDisabled(retrieveKeyChainResult);

            if (CapabilitiesEditor.EmbeddedCredentialsDisabled)
            {
                LogCapabilityDisabled("Embedded Credentials", Localizer[SharedResourceKeys.EmbeddedCredsDisabledReason]);
            }

            return Result.Succeeded();
        }

        private async Task<IResult<IDestinationSiteInfo>> GetDestinationSiteInfoAsync(CancellationToken cancel)
        {
            if (_destinationConfig == null)
            {
                var configNullError = new InvalidOperationException(Localizer[SharedResourceKeys.DestinationEndpointNotAnApiMsg]);

                return (IResult<IDestinationSiteInfo>)Result.FromErrors([configNullError]);
            }

            var sessionInfoResult = await _migration
                .Destination
                .GetSessionAsync(cancel)
                .ConfigureAwait(false);

            if (!sessionInfoResult.Success)
            {
                return sessionInfoResult.CastFailure<IDestinationSiteInfo>();
            }

            var connectionConfig = _destinationConfig.SiteConnectionConfiguration;

            return Result<IDestinationSiteInfo>.Succeeded(
                new DestinationSiteInfo(
                    connectionConfig.SiteContentUrl,
                    sessionInfoResult.Value.Site.Id,
                    connectionConfig.ServerUrl.AbsoluteUri));
        }

        /// <summary>
        /// Retrieves a fake keychain from the source endpoint. This is used to check if the embedded credential 
        /// migration is disabled, based on the return code.
        /// </summary>
        /// <param name="destinationSiteInfo">Destination Info.</param>
        /// <param name="cancel">Cancellation Token to obey.</param>
        /// <returns>Result of the fake RetrieveKeychain call.</returns>
        private async Task<IResult<IEmbeddedCredentialKeychainResult>> RetrieveKeychainAsync(
            IDestinationSiteInfo destinationSiteInfo,
            CancellationToken cancel)
        {
            var retrieveKeychainResult = await _migration
                .Source
                .RetrieveKeychainsAsync<IDataSource>(Guid.NewGuid(), destinationSiteInfo, cancel)
                .ConfigureAwait(false);

            if (!retrieveKeychainResult.Success)
            {
                return retrieveKeychainResult.CastFailure<IEmbeddedCredentialKeychainResult>();
            }

            return retrieveKeychainResult;
        }

        /// <summary>
        /// Checks if the embedded credential migration is disabled based on the errors returned from the keychain retrieval.
        /// </summary>
        /// <param name="retrieveKeyChainResult"></param>
        /// <returns></returns>
        private static bool IsEmbeddedCredentialMigrationDisabled(IResult<IEmbeddedCredentialKeychainResult> retrieveKeyChainResult)
            => retrieveKeyChainResult.Errors
                .Where(e => e is RestException)
                .Select(e => e as RestException)
                .Any(e => RestErrorCodes.Equals(e?.Code, RestErrorCodes.FEATURE_DISABLED));

    }
}
