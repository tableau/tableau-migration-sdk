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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a class representing the current authentication token.
    /// </summary>
    public interface IAuthenticationTokenProvider
    {
        /// <summary>
        /// Event that fires when an authentication token refresh is requested.
        /// </summary>
        event AsyncEventHandler? RefreshRequestedAsync;

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        string? Token { get; }

        /// <summary>
        /// Sets the authentication token.
        /// </summary>
        /// <param name="token">The authentication token received from the server.</param>
        void Set(string token);

        /// <summary>
        /// Clears the authentication token.
        /// </summary>
        void Clear();

        /// <summary>
        /// Requests an authentication token refresh.
        /// </summary>
        /// <param name="cancel"> A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task RequestRefreshAsync(CancellationToken cancel);
    }
}