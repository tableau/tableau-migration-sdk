namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for an object that has Tags.
    /// </summary>
    public interface IWithTagTypes
    {
        /// <summary>
        /// Gets the tags for the response.
        /// </summary>
        ITagType[]? Tags { get; internal set; }
    }
}
