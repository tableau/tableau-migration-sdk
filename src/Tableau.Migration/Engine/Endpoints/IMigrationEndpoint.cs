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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that represents a location to move Tableau data to or from.
    /// </summary>
    public interface IMigrationEndpoint : IAsyncDisposable
    {
        /// <summary>
        /// Performs pre-migration initialization.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>An awaitable task with the initialization result.</returns>
        Task<IResult> InitializeAsync(CancellationToken cancel);

        /// <summary>
        /// Gets a pager to list all the content the user has access to.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="pageSize">The page size to use.</param>
        /// <returns>A pager to list content with.</returns>
        IPager<TContent> GetPager<TContent>(int pageSize);

        /// <summary>
        /// Gets the current server session information.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>An awaitable task with the server session result.</returns>
        Task<IResult<IServerSession>> GetSessionAsync(CancellationToken cancel);
    }
}
