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

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a content typed API client that can list all of the content items the user has access to.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IPagedListApiClient<TContent> : IContentApiClient
    {
        /// <summary>
        /// Gets a pager to list all the content the user has access to.
        /// </summary>
        /// <param name="pageSize">The page size to use.</param>
        /// <returns>A pager to list content with.</returns>
        IPager<TContent> GetPager(int pageSize);

        /// <summary>
        /// Gets all the content items that the user has access to.
        /// </summary>
        /// <param name="pageSize">The expected maximum number of items to include in each page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The total results.</returns>
        public async Task<IResult<IImmutableList<TContent>>> GetAllAsync(int pageSize, CancellationToken cancel)
            => await GetPager(pageSize).GetAllPagesAsync(cancel).ConfigureAwait(false);
    }
}
