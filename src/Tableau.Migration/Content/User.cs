﻿//
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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal sealed class User : UsernameContentBase, IUser
    {
        /// <inheritdoc/>
        public string FullName { get; set; }

        /// <inheritdoc/>
        public string Email { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string SiteRole { get; set; }

        /// <inheritdoc/>
        public UserAuthenticationType Authentication { get; set; }

        public User(UsersResponse.UserType response)
            : this(
                  response.Id,
                  Guard.AgainstNullEmptyOrWhiteSpace(Guard.AgainstNull(response.Domain, () => response.Domain).Name, () => response.Domain.Name),
                  response.Email,
                  response.Name,
                  response.FullName,
                  response.SiteRole,
                  response.GetAuthenticationType()
                )
        { }

        public User(Guid id, IUpdateUserResult result)
            : this(id, null, result.Email, result.Name, result.FullName, result.SiteRole, result.Authentication)
        { }

        private User(
            Guid id,
            string? userDomain,
            string? email,
            string? name,
            string? fullName,
            string? siteRole,
            UserAuthenticationType authentication)
        {
            Id = Guard.AgainstDefaultValue(id, () => id);
            Email = email ?? string.Empty;
            Name = Guard.AgainstNullEmptyOrWhiteSpace(name, () => name);
            FullName = fullName ?? string.Empty;
            SiteRole = Guard.AgainstNullEmptyOrWhiteSpace(siteRole, () => siteRole);
            Authentication = authentication;
            Domain = userDomain ?? string.Empty;
        }
    }
}
