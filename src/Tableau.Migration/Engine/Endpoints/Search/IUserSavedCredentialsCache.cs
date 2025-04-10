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
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Interface for user saved credential cache.
    /// </summary>
    public interface IUserSavedCredentialsCache
    {
        /// <summary>
        /// Add or update saved credentials for the given user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="savedCredentials">Saved credentials for the user ID.</param>
        /// <returns>The value just added to the cache.</returns>
        IEmbeddedCredentialKeychainResult AddOrUpdate(Guid userId, IEmbeddedCredentialKeychainResult savedCredentials);

        /// <summary>
        /// Get the saved credentials for the given user ID if present in the cache.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The saved credentials if already cached else null.</returns>
        IEmbeddedCredentialKeychainResult? Get(Guid userId);
    }
}
