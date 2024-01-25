namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a mappable content item that belongs to a project/container.
    /// </summary>
    public interface IMappableContainerContent
    {
        /// <summary>
        /// Gets the current project/container the content item belongs to.
        /// Null if the content item is a top-level content item (e.g. top-level projects).
        /// </summary>
        IContentReference? Container { get; }

        /// <summary>
        /// Sets the content location, performing any renames as required.
        /// </summary>
        /// <param name="container">The new project/container to use.</param>
        /// <param name="newLocation">The new location to use.</param>
        void SetLocation(IContentReference? container, ContentLocation newLocation);
    }
}
