namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface for an object that can resolve file store paths from content items.
    /// </summary>
    public interface IContentFilePathResolver
    {
        /// <summary>
        /// Resolves a relative file store path for the content item.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="contentItem">the content item to resolve the path for.</param>
        /// <param name="originalFileName">The original file name.</param>
        /// <returns>The resolved relative file store path.</returns>
        string ResolveRelativePath<TContent>(TContent contentItem, string originalFileName);
    }
}
