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
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for an API client sign-in result model. 
    /// </summary>
    internal class SignInResult : ISignInResult
    {
        /// <inheritdoc/>
        public string Token { get; }

        /// <inheritdoc/>
        public Guid SiteId { get; }

        /// <inheritdoc/>
        public string SiteContentUrl { get; }

        /// <inheritdoc/>
        public Guid UserId { get; }

        /// <summary>
        /// Creates a new <see cref="SignInResult"/> instance.
        /// </summary>
        /// <param name="response">The REST API sign-in response.</param>
        public SignInResult(SignInResponse response)
        {
            var credentials = Guard.AgainstNull(response.Item, () => response.Item);
            var site = Guard.AgainstNull(response.Item.Site, () => response.Item.Site);
            var user = Guard.AgainstNull(response.Item.User, () => response.Item.User);

            Token = Guard.AgainstNullEmptyOrWhiteSpace(credentials.Token, () => response.Item.Token);
            SiteId = Guard.AgainstDefaultValue(site.Id, () => response.Item.Site.Id);
            SiteContentUrl = Guard.AgainstNullOrWhiteSpace(site.ContentUrl, () => response.Item.Site.ContentUrl);
            UserId = Guard.AgainstDefaultValue(user.Id, () => response.Item.User.Id);
        }
    }
}
