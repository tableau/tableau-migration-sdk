namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for content items with a domain.
    /// </summary>
    public interface IWithDomain
    {
        /// <summary>
        /// Gets the domain this item belongs to.
        /// Changes should be made through mapping.
        /// </summary>
        string Domain { get; }
    }
}