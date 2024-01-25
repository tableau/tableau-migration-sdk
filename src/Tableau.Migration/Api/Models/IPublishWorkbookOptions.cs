using System;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for API client workbook publish options. 
    /// </summary>
    public interface IPublishWorkbookOptions : IPublishFileOptions
    {
        /// <summary>
        /// Gets the name of the workbook.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the description of the workbook.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets whether to show tabs for the workbook.
        /// </summary>
        bool ShowTabs { get; }

        /// <summary>
        /// Gets whether to encrypt extracts for the workbook.
        /// </summary>
        bool EncryptExtracts { get; }

        /// <summary>
        /// Gets whether or not to skip the data source connection check.
        /// </summary>
        bool SkipConnectionCheck { get; }

        /// <summary>
        /// Gets whether or not to overwrite any existing workbook.
        /// </summary>
        bool Overwrite { get; }

        /// <summary>
        /// Gets the ID of the user to generate thumbnails as.
        /// </summary>
        Guid? ThumbnailsUserId { get; }

        /// <summary>
        /// Gets the ID of the project to publish to.
        /// </summary>
        Guid ProjectId { get; }
    }
}
