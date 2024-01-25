namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface to be inherited by content items with owner.
    /// </summary>
    public interface IWithOwner : IContentReference
    {
        /// <summary>
        /// Gets or sets the owner for the content item.
        /// </summary>
        IContentReference Owner { get; set; }
    }
}