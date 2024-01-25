namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// Interface for a content object that has a name.
    /// </summary>
    public interface INamedContent
    {
        /// <summary>
        /// Gets the content item's name
        /// </summary>
        string? Name { get; }
    }
}
