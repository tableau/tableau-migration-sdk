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

namespace Tableau.Migration.Paging
{
    /// <summary>
    /// Abstract <see cref="IPager{TContent}"/> implementation uses an indexed page number.
    /// Defaults to 1-based index number as that is the indexing used in Tableau REST APIs.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public abstract class IndexedPagerBase<TContent> : IPager<TContent>
    {
        private readonly int _pageSize;

        private int _pageNumber;

        /// <summary>
        /// Creates a new <see cref="IndexedPagerBase{TContent}"/> object.
        /// </summary>
        /// <param name="pageSize">The page size to page by.</param>
        /// <param name="defaultPageNumber">The default page number to index on.</param>
        public IndexedPagerBase(int pageSize, int defaultPageNumber = 1)
        {
            _pageSize = pageSize;
            _pageNumber = defaultPageNumber;
        }

        /// <summary>
        /// Retrieves a page of data for a given page number.
        /// </summary>
        /// <param name="pageNumber">The page number to get data for.</param>
        /// <param name="pageSize">The size of the page of data to get.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The paged results.</returns>
        protected abstract Task<IPagedResult<TContent>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel);

        /// <inheritdoc />
        public async Task<IPagedResult<TContent>> NextPageAsync(CancellationToken cancel)
            => await GetPageAsync(_pageNumber++, _pageSize, cancel).ConfigureAwait(false);
    }
}
