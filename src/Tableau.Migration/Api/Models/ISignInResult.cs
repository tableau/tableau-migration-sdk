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

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for an API client sign-in result model. 
    /// </summary>
    public interface ISignInResult
    {
        /// <summary>
        /// Gets the authentication token for the sign-in.
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Gets the ID for the signed-in site.
        /// </summary>
        Guid SiteId { get; }

        /// <summary>
        /// Gets the content URL for the signed-in site.
        /// </summary>
        string SiteContentUrl { get; }

        /// <summary>
        /// Gets the ID for the signed-in user.
        /// </summary>
        Guid UserId { get; }
    }
}