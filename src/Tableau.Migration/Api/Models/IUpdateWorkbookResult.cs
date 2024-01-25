using System;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface to represent the response returned by the Update method in <see cref="IWorkbooksApiClient"/>.
    /// </summary>
    public interface IUpdateWorkbookResult
    {
        /// <summary>
        /// Gets the unique identifier of the workbook.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the workbook.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the description of the workbook.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the content URL of the workbook.
        /// </summary>
        string? ContentUrl { get; }

        /// <summary>
        /// Gets the show tabs option for the workbook.
        /// </summary>
        bool ShowTabs { get; }

        /// <summary>
        /// Gets the creation date/time of the workbook.
        /// </summary>
        DateTime CreatedAtUtc { get; }

        /// <summary>
        /// Gets the update date/time of the workbook.
        /// </summary>
        DateTime UpdatedAtUtc { get; }

        /// <summary>
        /// Gets the encrypt extracts flag for the workbook.
        /// </summary>
        bool EncryptExtracts { get; }
    }
}
