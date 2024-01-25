namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a view REST response.
    /// </summary>
    public interface IViewsType : IRestIdentifiable, INamedContent, IWithOwnerType, IWithWorkbookReferenceType
    {
        /// <summary>
        /// The content URL for the response.
        /// </summary>
        public string? ContentUrl { get; }
    }
}
