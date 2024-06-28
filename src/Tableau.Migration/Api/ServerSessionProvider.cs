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

using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api
{
    internal sealed class ServerSessionProvider : IServerSessionProvider
    {
        private readonly ITableauServerVersionProvider _versionProvider;
        private readonly IAuthenticationTokenProvider _tokenProvider;

        internal async Task<string?> GetAuthenticationTokenAsync(CancellationToken cancel)
            => await _tokenProvider.GetAsync(cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public TableauServerVersion? Version => _versionProvider.Version;

        /// <inheritdoc />
        public string? SiteContentUrl { get; private set; }

        /// <inheritdoc />
        public Guid? SiteId { get; private set; }

        /// <inheritdoc />
        public Guid? UserId { get; private set; }

        /// <inheritdoc />
        public TableauInstanceType InstanceType { get; private set; }

        public ServerSessionProvider(
            ITableauServerVersionProvider versionProvider,
            IAuthenticationTokenProvider tokenProvider)
        {
            _versionProvider = versionProvider;
            _tokenProvider = tokenProvider;
        }

        /// <inheritdoc />
        public async Task SetCurrentSessionAsync(ISignInResult signInResult, TableauInstanceType instanceType, CancellationToken cancel)
            => await SetCurrentSessionAsync(
                signInResult.UserId,
                signInResult.SiteId,
                signInResult.SiteContentUrl,
                signInResult.Token,
                instanceType,
                cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task SetCurrentSessionAsync(
            Guid userId,
            Guid siteId,
            string siteContentUrl,
            string authenticationToken,
            TableauInstanceType instanceType,
            CancellationToken cancel)
        {
            SiteId = siteId;
            SiteContentUrl = siteContentUrl;
            UserId = userId;
            InstanceType = instanceType;

            await _tokenProvider.SetAsync(authenticationToken, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ClearCurrentSessionAsync(CancellationToken cancel)
        {
            SiteId = null;
            SiteContentUrl = null;
            UserId = null;
            InstanceType = TableauInstanceType.Unknown;

            await _tokenProvider.ClearAsync(cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void SetVersion(TableauServerVersion version) => _versionProvider.Set(version);
    }
}
