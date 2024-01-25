namespace Tableau.Migration.Content
{
    /// <summary>
    /// Inteface for tags associated with content items.
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Gets or sets label for the tag.
        /// </summary>
        string Label { get; set; }
    }
}