//
//  Copyright (c) 2026, Salesforce, Inc.
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Paging
{
    /// <summary>
    /// Interface for an object that can list a single page of the content items the user has access to with filters.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IApiFilteredPageAccessor<TContent> : IApiPageAccessor<TContent>
    {
        /// <summary>
        /// Gets a single page of the content items that the user has access to.
        /// </summary>
        /// <param name="filters">The filters to apply.</param>
        /// <param name="pageNumber">The 1-indexed page number for the page to list.</param>
        /// <param name="pageSize">The expected maximum number of items to include in the page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The paged results.</returns>
        Task<IPagedResult<TContent>> GetPageAsync(IEnumerable<Filter> filters, int pageNumber, int pageSize, CancellationToken cancel);
    }
}
