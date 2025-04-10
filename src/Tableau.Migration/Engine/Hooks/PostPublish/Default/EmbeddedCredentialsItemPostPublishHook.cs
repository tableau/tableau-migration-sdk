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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Embedded credential content post publish hook. 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public class EmbeddedCredentialsItemPostPublishHook<TPublish, TResult>
        : ContentItemPostPublishHookBase<TPublish, TResult>
        where TPublish : IRequiresEmbeddedCredentialMigration, IContentReference, IConnectionsContent
        where TResult : IWithEmbeddedCredentials
    {
        private readonly IMigration _migration;
        private readonly ITableauApiEndpointConfiguration? _destinationConfig; //Null if the destination is not an API.
        private readonly ILogger _logger;
        private readonly ISharedResourcesLocalizer _sharedResourcesLocalizer;
        private readonly IDestinationContentReferenceFinder<IUser> _userContentFinder;
        private readonly IUserSavedCredentialsCache _userSavedCredentialsCache;
        private readonly IMigrationCapabilities _migrationCapabilities;

        /// <summary>
        /// Creates a new <see cref="EmbeddedCredentialsItemPostPublishHook{TPublish, TDestination}"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="destinationFinderFactory">The destination content reference finder factory.</param>
        /// <param name="userSavedCredentialsCache">The cache for user saved credentials.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="sharedResourcesLocalizer">The <see cref="ISharedResourcesLocalizer"/>.</param>
        /// <param name="migrationCapabilities">The migration capabilities.</param>
        public EmbeddedCredentialsItemPostPublishHook(
            IMigration migration,
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            IUserSavedCredentialsCache userSavedCredentialsCache,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IMigrationCapabilities migrationCapabilities)
        {
            _migration = migration;
            _userContentFinder = destinationFinderFactory.ForDestinationContentType<IUser>();
            _destinationConfig = migration.Plan.Destination as ITableauApiEndpointConfiguration;
            _logger = loggerFactory.CreateLogger<EmbeddedCredentialsItemPostPublishHook<TPublish, TResult>>();
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
            _userSavedCredentialsCache = userSavedCredentialsCache;
            _migrationCapabilities = migrationCapabilities;
        }


        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            CancellationToken cancel)
        {
            if (_migrationCapabilities.EmbeddedCredentialsDisabled || !ctx.PublishedItem.HasEmbeddedPassword)
            {
                return ctx;
            }

            var destinationSiteInfo = await GetDestinationSiteInfoAsync(ctx, cancel).ConfigureAwait(false);

            if (!destinationSiteInfo.Success)
            {
                return ctx;
            }

            var retrieveKeychainResult = await RetrieveKeychainAsync(ctx, destinationSiteInfo.Value, cancel)
                .ConfigureAwait(false);

            if (!retrieveKeychainResult.Success)
            {
                return ctx;
            }

            var retrievedKeychains = retrieveKeychainResult.Value;

            if (retrievedKeychains.EncryptedKeychains.Count == 0)
            {
                return ctx;
            }

            var keychainUserMapping = await GetKeychainUserMappingAsync(ctx, retrievedKeychains.AssociatedUserIds, cancel)
                .ConfigureAwait(false);

            if (!keychainUserMapping.Success)
            {
                return ctx;
            }

            var keychainApplied = await ApplyKeyChainAsync(ctx, retrievedKeychains.EncryptedKeychains, keychainUserMapping.Value, cancel)
                .ConfigureAwait(false);

            if (!keychainApplied.Success)
            {
                return ctx;
            }

            foreach (var item in keychainUserMapping.Value)
            {
                var userSavedCredsResult = await RetrieveUserSavedCredentialsAsync(ctx, item.SourceUserId, destinationSiteInfo.Value, cancel)
                    .ConfigureAwait(false);

                if (!userSavedCredsResult.Success)
                {
                    return ctx;
                }

                var uploadSavedCredsResult = await UploadUserSavedCredentials(ctx, item.DestinationUserId, userSavedCredsResult.Value, cancel)
                    .ConfigureAwait(false);

                if (!uploadSavedCredsResult.Success)
                {
                    return ctx;
                }
            }

            LogManagedOAuthCredentialMigrationWarning(ctx.PublishedItem);

            return ctx;
        }

        private async Task<IResult<IDestinationSiteInfo>> GetDestinationSiteInfoAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            CancellationToken cancel)
        {
            if (_destinationConfig == null)
            {
                var configNullError = new InvalidOperationException(
                    _sharedResourcesLocalizer[SharedResourceKeys.DestinationEndpointNotAnApiMsg]);

                ctx.ManifestEntry.SetFailed(configNullError);

                return (IResult<IDestinationSiteInfo>)Result.FromErrors([configNullError]);
            }

            var sessionInfoResult = await _migration
                .Destination
                .GetSessionAsync(cancel)
                .ConfigureAwait(false);

            if (!sessionInfoResult.Success)
            {
                ctx.ManifestEntry.SetFailed(sessionInfoResult.Errors);
                return sessionInfoResult.CastFailure<IDestinationSiteInfo>();
            }

            var connectionConfig = _destinationConfig.SiteConnectionConfiguration;

            return Result<IDestinationSiteInfo>.Succeeded(
                new DestinationSiteInfo(
                    connectionConfig.SiteContentUrl,
                    sessionInfoResult.Value.Site.Id,
                    connectionConfig.ServerUrl.AbsoluteUri));
        }

        private async Task<IResult<IEmbeddedCredentialKeychainResult>> RetrieveKeychainAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            IDestinationSiteInfo destinationSiteInfo,
            CancellationToken cancel)
        {
            var retrieveKeychainResult = await _migration
                .Source
                .RetrieveKeychainsAsync<TPublish>(ctx.PublishedItem.Id, destinationSiteInfo, cancel)
                .ConfigureAwait(false);

            if (!retrieveKeychainResult.Success)
            {
                ctx.ManifestEntry.SetFailed(retrieveKeychainResult.Errors);
                return retrieveKeychainResult.CastFailure<IEmbeddedCredentialKeychainResult>();
            }

            return retrieveKeychainResult;
        }

        private async Task<IResult<List<IKeychainUserMapping>>> GetKeychainUserMappingAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            IImmutableList<Guid> associatedUserIds,
            CancellationToken cancel)
        {
            var keychainUserMapping = new List<IKeychainUserMapping>();

            if (!associatedUserIds.Any())
            {
                return Result<List<IKeychainUserMapping>>.Succeeded(keychainUserMapping);
            }

            var userIdsWithNoDestination = new List<string>();

            foreach (var sourceUserId in associatedUserIds)
            {
                var destinationUser = await _userContentFinder.FindBySourceIdAsync(sourceUserId, cancel)
                    .ConfigureAwait(false);

                if (destinationUser == null)
                {
                    userIdsWithNoDestination.Add(sourceUserId.ToString());
                    continue;
                }

                keychainUserMapping.Add(new KeychainUserMapping(sourceUserId, destinationUser.Id));
            }

            LogAssociatedUserIdsWithNoDestination(ctx.PublishedItem, userIdsWithNoDestination);

            return Result<List<IKeychainUserMapping>>.Succeeded(keychainUserMapping);
        }

        private void LogAssociatedUserIdsWithNoDestination(TPublish publishedItem, List<string> userIdsWithNoDestination)
        {
            if (userIdsWithNoDestination.Count == 0)
            {
                return;
            }

            _logger.LogWarning(
              _sharedResourcesLocalizer[SharedResourceKeys.OAuthCredentialMigrationUsersNotAtDestination],
              publishedItem.Name,
              publishedItem.ContentUrl,
              string.Join(',', userIdsWithNoDestination));
        }

        private async Task<IResult> ApplyKeyChainAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            IImmutableList<string> encryptedKeychains,
            List<IKeychainUserMapping> keychainUserMapping,
            CancellationToken cancel)
        {
            var applyKeychainResult = await _migration.Destination
               .ApplyKeychainsAsync<TPublish>(
               ctx.DestinationItem.Id,
               new ApplyKeychainOptions(encryptedKeychains, keychainUserMapping),
               cancel)
               .ConfigureAwait(false);

            if (!applyKeychainResult.Success)
            {
                ctx.ManifestEntry.SetFailed(applyKeychainResult.Errors);
                return Result.Failed(applyKeychainResult.Errors);
            }

            return Result.Succeeded();
        }

        private void LogManagedOAuthCredentialMigrationWarning(TPublish publishedItem)
        {
            if (!publishedItem.HasEmbeddedOAuthManagedKeychain)
            {
                return;
            }

            var connectionIds = publishedItem.Connections.Where(c => c.UseOAuthManagedKeychain == true).Select(c => c.Id.ToString());

            if (!connectionIds.Any())
            {
                return;
            }

            _logger.LogWarning(
                _sharedResourcesLocalizer[SharedResourceKeys.HasManagedOAuthCredentialsWarning],
                publishedItem.GetType().Name,
                publishedItem.Name,
                publishedItem.ContentUrl,
                string.Join(',', connectionIds));

            return;
        }

        private async Task<IResult> UploadUserSavedCredentials(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            Guid userId,
            IEmbeddedCredentialKeychainResult userSavedCreds,
            CancellationToken cancel)
        {
            if (userSavedCreds.EncryptedKeychains.Count == 0)
            {
                return Result.Succeeded();
            }

            var result = await _migration
                .Destination
                .UploadUserSavedCredentialsAsync(userId, userSavedCreds.EncryptedKeychains, cancel)
                .ConfigureAwait(false);

            if (!result.Success)
            {
                ctx.ManifestEntry.SetFailed(result.Errors);
                return result;
            }

            return Result.Succeeded();
        }

        private async Task<IResult<IEmbeddedCredentialKeychainResult>> RetrieveUserSavedCredentialsAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            Guid userId,
            IDestinationSiteInfo destinationSiteInfo,
            CancellationToken cancel)
        {
            var cachedResult = _userSavedCredentialsCache.Get(userId);
            if (cachedResult != null)
            {
                return Result<IEmbeddedCredentialKeychainResult>.Succeeded(cachedResult);
            }

            var result = await _migration
                .Source
                .RetrieveUserSavedCredentialsAsync(userId, destinationSiteInfo, cancel).ConfigureAwait(false);

            if (!result.Success)
            {
                ctx.ManifestEntry.SetFailed(result.Errors);
                return result;
            }

            _userSavedCredentialsCache.AddOrUpdate(userId, result.Value);

            return result;
        }
    }
}
