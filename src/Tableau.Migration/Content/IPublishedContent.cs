namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item that has metadata around publishing information.
    /// </summary>
    public interface IPublishedContent
    {
        /// <summary>
        /// Gets the created timestamp.
        /// </summary>
        string CreatedAt { get; }

        /// <summary>
        /// Gets the updated timestamp.
        /// </summary>
        string? UpdatedAt { get; }

        /// <summary>
        /// Gets the webpage URL.
        /// </summary>
        string? WebpageUrl { get; }
    }
}
