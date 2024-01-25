namespace Tableau.Migration.Api.Tags
{
    /// <summary>
    /// Interface for an API client 
    /// for a content type that has tag operations.
    /// </summary>
    public interface ITagsContentApiClient
    {
        /// <summary>
        /// Gets the tags API client.
        /// </summary>
        ITagsApiClient Tags { get; }
    }
}
