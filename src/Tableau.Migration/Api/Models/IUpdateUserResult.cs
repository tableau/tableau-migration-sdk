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

using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface to represent the response returned by the Update method in <see cref="IUsersApiClient"/>.
    /// </summary>
    public interface IUpdateUserResult
    {
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        string? FullName { get; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Gets the site role of the user.
        /// </summary>
        string SiteRole { get; }

        /// <summary>
        /// The authentication for the user.
        /// </summary>
        UserAuthenticationType Authentication { get; }
    }
}