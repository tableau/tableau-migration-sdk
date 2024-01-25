﻿using System;
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
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The paged results.</returns>
        Task<IPagedResult<TContent>> NextPageAsync(CancellationToken cancel);

        /// <summary>
        /// Processes all pages of content.
        /// </summary>
        /// <param name="initCapacity">An action to take once the first page is loaded and the total needed capacity is known.</param>
        /// <param name="pageAction">An action to take on each page.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The combined results.</returns>
        public async Task<IResult> GetAllPagesAsync(Action<int> initCapacity, Action<IImmutableList<TContent>> pageAction, CancellationToken cancel)
        {
            var resultBuilder = new ResultBuilder();

            var pagedResults = await NextPageAsync(cancel).ConfigureAwait(false);
            resultBuilder.Add(pagedResults);

            initCapacity(pagedResults.TotalCount);

            var foundCount = 0;
            while (!pagedResults.Value.IsNullOrEmpty())
            {
                cancel.ThrowIfCancellationRequested();

                pageAction(pagedResults.Value);
                foundCount += pagedResults.Value.Count;

                if (foundCount == pagedResults.TotalCount)
                    break;

                pagedResults = await NextPageAsync(cancel).ConfigureAwait(false);
                resultBuilder.Add(pagedResults);
            }

            return resultBuilder.Build();
        }

        /// <summary>
        /// Combines all pages of content.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The combined results.</returns>
        public async Task<IResult<IImmutableList<TContent>>> GetAllPagesAsync(CancellationToken cancel)
        {
            var resultItems = ImmutableArray.CreateBuilder<TContent>();

            var result = await GetAllPagesAsync(
                capacity => resultItems.Capacity = capacity,
                page => resultItems.AddRange(page),
                cancel).ConfigureAwait(false);

            return Result<IImmutableList<TContent>>.Create(result, resultItems.ToImmutable());
        }
    }
}
