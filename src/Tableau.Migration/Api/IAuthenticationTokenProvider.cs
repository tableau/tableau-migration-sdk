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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a thread safe class representing the current authentication token.
    /// </summary>
    public interface IAuthenticationTokenProvider
    {
        /// <summary>
        /// Event that fires when an authentication token refresh is requested.
        /// </summary>
        event RefreshAuthenticationTokenDelegate? RefreshRequestedAsync;

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await for the current authentication token.</returns>
        Task<string?> GetAsync(CancellationToken cancel);

        /// <summary>
        /// Sets the authentication token.
        /// </summary>
        /// <param name="token">The authentication token received from the server.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The task to await.</returns>
        Task SetAsync(string token, CancellationToken cancel);

        /// <summary>
        /// Clears the authentication token.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The task to await.</returns>
        Task ClearAsync(CancellationToken cancel);

        /// <summary>
        /// Requests an authentication token refresh.
        /// </summary>
        /// <param name="previousToken">
        /// The token that was previously used before the refresh was requested.
        /// Used to de-duplicate refresh requests.
        /// </param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The task to await.</returns>
        Task RequestRefreshAsync(string? previousToken, CancellationToken cancel);
    }
}