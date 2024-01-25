namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Interface for an object that can create content reference finders
    /// based on content type.
    /// </summary>
    public interface IContentReferenceFinderFactory
    {
        /// <summary>
        /// Gets or creates a content reference finder for a given content type. 
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The content reference finder.</returns>
        IContentReferenceFinder<TContent> ForContentType<TContent>()
            where TContent : IContentReference;
    }
}
