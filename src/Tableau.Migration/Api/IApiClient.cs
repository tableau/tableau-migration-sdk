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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for unauthenticated API clients.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Signs into Tableau Server.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>An authenticated <see cref="ISitesApiClient"/>.</returns>
        Task<IAsyncDisposableResult<ISitesApiClient>> SignInAsync(CancellationToken cancel);

        /// <summary>
        /// Gets the version information for the Tableau Server.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The information for the current Tableau Server.</returns>
        Task<IResult<IServerInfo>> GetServerInfoAsync(CancellationToken cancel);

        /// <summary>
        /// Gets the current session information.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The session information.</returns>
        Task<IResult<IServerSession>> GetCurrentServerSessionAsync(CancellationToken cancel);
    }
}
