// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api
{
    internal sealed class ServerSessionProvider : IServerSessionProvider
    {
        private readonly ITableauServerVersionProvider _versionProvider;
        private readonly IAuthenticationTokenProvider _tokenProvider;

        public TableauServerVersion? Version => _versionProvider.Version;
        public string? AuthenticationToken => _tokenProvider.Token;

        public Guid? SiteId { get; private set; }
        public string? SiteContentUrl { get; private set; }

        public Guid? UserId { get; private set; }

        public ServerSessionProvider(
            ITableauServerVersionProvider versionProvider,
            IAuthenticationTokenProvider tokenProvider)
        {
            _versionProvider = versionProvider;
            _tokenProvider = tokenProvider;
        }

        public void SetCurrentUserAndSite(ISignInResult signInResult)
            => SetCurrentUserAndSite(signInResult.UserId, signInResult.SiteId, signInResult.SiteContentUrl, signInResult.Token);

        public void SetCurrentUserAndSite(Guid userId, Guid siteId, string siteContentUrl, string authenticationToken)
        {
            SiteId = siteId;
            SiteContentUrl = siteContentUrl;
            UserId = userId;

            _tokenProvider.Set(authenticationToken);
        }

        public void ClearCurrentUserAndSite()
        {
            SiteId = null;
            SiteContentUrl = null;
            UserId = null;

            _tokenProvider.Clear();
        }

        public void SetVersion(TableauServerVersion version) => _versionProvider.Set(version);
    }
}
