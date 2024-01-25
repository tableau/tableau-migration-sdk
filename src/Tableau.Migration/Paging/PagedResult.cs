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
        /// <param name="errors">The errors encountered during the operation, if any.</param>
        protected PagedResult(bool success, IImmutableList<TItem>? value, int pageNumber, int pageSize, int totalCount, params Exception[] errors)
            : base(success, value, errors)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        /// <inheritdoc />
        public int PageNumber { get; }

        /// <inheritdoc />
        public int PageSize { get; }

        /// <inheritdoc />
        public int TotalCount { get; }

        /// <summary>
        /// Creates a new <see cref="PagedResult{TItem}"/> instance for successful paged operations.
        /// </summary>
        /// <param name="value">The result of the operation.</param>
        /// <param name="pageNumber">The current 1-indexed page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total unpaged available item count.</param>
        /// <returns>A new <see cref="PagedResult{TItem}"/> instance.</returns>
        public static PagedResult<TItem> Succeeded(IImmutableList<TItem> value, int pageNumber, int pageSize, int totalCount)
            => new(true, value, pageNumber, pageSize, totalCount);

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
        public static new PagedResult<TItem> Failed(IEnumerable<Exception> errors) => new(false, null, 0, 0, 0, errors.ToArray());
    }
}
