using System;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a workbook REST response.
    /// </summary>
    public interface IWorkbookType : IRestIdentifiable, INamedContent, IWithProjectType, IWithOwnerType, IWithTagTypes
    {
        /// <summary>
        /// The description for the response.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// The content URL for the response.
        /// </summary>
        public string? ContentUrl { get; }

        /// <summary>
        /// whether tabs are shown for the response.
        /// </summary>
        public bool ShowTabs { get; }

        /// <summary>
        /// The size for the response.
        /// </summary>        
        public long Size { get; }

        /// <summary>
        /// The webpage URL for the response.
        /// </summary>       
        public string? WebpageUrl { get; }

        /// <summary>
        /// The created timestamp for the response.
        /// </summary>        
        public string? CreatedAt { get; }

        /// <summary>
        /// The updated timestamp for the response.
        /// </summary>
        public string? UpdatedAt { get; }

        /// <summary>
        /// Gets the encrypted extracts flag for the response.
        /// </summary>
        bool EncryptExtracts { get; }

        /// <summary>
        /// The Default View Id for the response.
        /// </summary>
        Guid DefaultViewId { get; }

        /// <summary>
        /// The location for the response.
        /// </summary>
        ILocationType? Location { get; }
    }
}
