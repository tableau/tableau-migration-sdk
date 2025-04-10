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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api.EmbeddedCredentials
{
    /// <summary>
    /// Interface for an API client that modifies content's embedded credentials.
    /// </summary>
    public interface IEmbeddedCredentialsApiClient
    {
        /// <summary>
        /// Retrieves the encrypted keychains for the content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="destinationSiteInfo">The destination site information.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The operation result.</returns>
        Task<IResult<IEmbeddedCredentialKeychainResult>> RetrieveKeychainAsync(
            Guid contentItemId,
            IDestinationSiteInfo destinationSiteInfo,
            CancellationToken cancel);

        /// <summary>
        /// Uploads and applies encrypted keychains to a content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="options">The apply keychain options.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The operation result.</returns>
        Task<IResult> ApplyKeychainAsync(Guid contentItemId, IApplyKeychainOptions options, CancellationToken cancel);
    }
}