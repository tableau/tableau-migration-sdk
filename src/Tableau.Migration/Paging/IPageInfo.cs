namespace Tableau.Migration.Paging
{
    /// <summary>
    /// Interface for an object that describes information about a page of data.
    /// </summary>
    public interface IPageInfo
    {
        /// <summary>
        /// Gets the 1-indexed page number for the current page.
        /// </summary>
        int PageNumber { get; }

        /// <summary>
        /// Gets the expected maximum size of the page.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the total unpaged item count.
        /// </summary>
        int TotalCount { get; }
    }
}
