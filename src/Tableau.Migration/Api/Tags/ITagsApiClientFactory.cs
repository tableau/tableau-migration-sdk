namespace Tableau.Migration.Api.Tags
{
    /// <summary>
    /// Interface for an object that can create <see cref="ITagsApiClient"/> objects.
    /// </summary>
    public interface ITagsApiClientFactory
    {
        /// <summary>
        /// Creates an <see cref="ITagsApiClient"/> instance.
        /// </summary>
        /// <param name="contentApiClient">The content API client to use to determine the URL prefix.</param>
        /// <returns>The created <see cref="ITagsApiClient"/>.</returns>
        ITagsApiClient Create(IContentApiClient contentApiClient);
    }
}
