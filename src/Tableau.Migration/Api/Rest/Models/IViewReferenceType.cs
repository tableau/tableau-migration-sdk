namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface representing an XML element for the view of a content item.
    /// </summary>
    public interface IViewReferenceType : IRestIdentifiable, INamedContent
    {
        /// <summary>
        /// The content URL for the view response.
        /// </summary>
        string? ContentUrl { get; }

        /// <summary>
        /// The tags for the response.
        /// </summary>
        ITagType[] Tags { get; }
    }
}
