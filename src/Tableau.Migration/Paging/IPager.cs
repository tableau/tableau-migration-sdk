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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Paging
{
    /// <summary>
    /// Interface for an object that can retrieve pages of content.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IPager<TContent>
    {
        /// <summary>
        /// Gets a page of content.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The paged results.</returns>
        Task<IPagedResult<TContent>> NextPageAsync(CancellationToken cancel);

        /// <summary>
        /// Processes all pages of content.
        /// </summary>
        /// <param name="initCapacity">An action to take once the first page is loaded and the total needed capacity is known.</param>
        /// <param name="pageAction">An action to take on each page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The combined results.</returns>
        public async Task<IResult> GetAllPagesAsync(Action<int> initCapacity, Action<IImmutableList<TContent>> pageAction, CancellationToken cancel)
        {
            var resultBuilder = new ResultBuilder();

            var pagedResults = await NextPageAsync(cancel).ConfigureAwait(false);
            resultBuilder.Add(pagedResults);

            initCapacity(pagedResults.TotalCount);

            while (!pagedResults.FetchedAllPages)
            {
                cancel.ThrowIfCancellationRequested();

                ExecutePageAction(pagedResults);

                pagedResults = await NextPageAsync(cancel).ConfigureAwait(false);
                resultBuilder.Add(pagedResults);
            }

            ExecutePageAction(pagedResults);

            void ExecutePageAction(IPagedResult<TContent> results)
            {
                if (!results.Value.IsNullOrEmpty())
                {
                    pageAction(results.Value);
                }
            }

            return resultBuilder.Build();
        }

        /// <summary>
        /// Combines all pages of content.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The combined results.</returns>
        public async Task<IResult<IImmutableList<TContent>>> GetAllPagesAsync(CancellationToken cancel)
        {
            var resultItems = ImmutableArray.CreateBuilder<TContent>();

            var result = await GetAllPagesAsync(
                capacity => resultItems.Capacity = capacity,
                resultItems.AddRange,
                cancel).ConfigureAwait(false);

            return Result<IImmutableList<TContent>>.Create(result, resultItems.ToImmutable());
        }
    }
}
