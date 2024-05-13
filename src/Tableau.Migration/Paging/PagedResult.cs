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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tableau.Migration.Paging
{
    internal record PagedResult<TItem> : Result<IImmutableList<TItem>>, IPagedResult<TItem>
    {
        /// <summary>
        /// Creates a new <see cref="PagedResult{TItem}"/> object.
        /// </summary>
        /// <param name="success">True if the operation is successful, false otherwise.</param>
        /// <param name="value">The paged result of the operation.</param>
        /// <param name="pageNumber">The current 1-indexed page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total unpaged available item count.</param>
        /// <param name="fetchedAllPages">Whether the SDK has already fetched all pages or not.</param>
        /// <param name="errors">The errors encountered during the operation, if any.</param>
        protected PagedResult(bool success, IImmutableList<TItem>? value, int pageNumber, int pageSize, int totalCount, bool fetchedAllPages, params Exception[] errors)
            : base(success, value, errors)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            FetchedAllPages = fetchedAllPages;
        }

        /// <inheritdoc />
        public int PageNumber { get; }

        /// <inheritdoc />
        public int PageSize { get; }

        /// <inheritdoc />
        public int TotalCount { get; }

        /// <inheritdoc />
        public bool FetchedAllPages { get; }

        /// <summary>
        /// Creates a new <see cref="PagedResult{TItem}"/> instance for successful paged operations.
        /// </summary>
        /// <param name="value">The result of the operation.</param>
        /// <param name="pageNumber">The current 1-indexed page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total unpaged available item count.</param>
        /// <param name="fetchedAllPages">Whether the SDK has already fetched all pages or not.</param>
        /// <returns>A new <see cref="PagedResult{TItem}"/> instance.</returns>
        public static PagedResult<TItem> Succeeded(IImmutableList<TItem> value, int pageNumber, int pageSize, int totalCount, bool fetchedAllPages)
            => new(true, value, pageNumber, pageSize, totalCount, fetchedAllPages);

        /// <summary>
        /// Creates a new <see cref="PagedResult{T}"/> instance for failed operations.
        /// </summary>
        /// <param name="error">The error encountered during the operation.</param>
        /// <returns>A new <see cref="PagedResult{T}"/> instance.</returns>
        public static new PagedResult<TItem> Failed(Exception error) => Failed(new[] { error });

        /// <summary>
        /// Creates a new <see cref="PagedResult{TItem}"/> instance for failed paged operations.
        /// </summary>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <returns>A new <see cref="PagedResult{TItem}"/> instance.</returns>
        public static new PagedResult<TItem> Failed(IEnumerable<Exception> errors) => new(false, null, 0, 0, 0, true, errors.ToArray());
    }
}
