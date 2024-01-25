namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface representing an XML element for the tag of a content item.
    /// </summary>
    public interface ITagType
    {
        /// <summary>
        /// The tag label for the response.
        /// </summary>
        string? Label { get; set; }
    }
}
