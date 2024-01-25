namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item that belongs to a container (e.g. project or personal space).
    /// </summary>
    public interface IContainerContent
    {
        /// <summary>
        /// Gets the container for the content item.
        /// Relocating the content should be done through mapping.
        /// </summary>
        IContentReference Container { get; }
    }
}
