namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a view REST response.
    /// </summary>
    public interface IViewType : IRestIdentifiable, INamedContent, IWithTagTypes, IWithWorkbookReferenceType
    {
        /// <summary>
        /// The content URL for the response.
        /// </summary>
        public string? ContentUrl { get; }

        /// <summary>
        /// The created timestamp for the response.
        /// </summary>        
        public string? CreatedAt { get; }

        /// <summary>
        /// The updated timestamp for the response.
        /// </summary>
        public string? UpdatedAt { get; }

        /// <summary>
        /// The View URL Name for the response.
        /// </summary>
        public string? ViewUrlName { get; }
    }
}
