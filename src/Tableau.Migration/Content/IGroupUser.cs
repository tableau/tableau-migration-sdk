namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a user linked to a group content item.
    /// </summary>
    public interface IGroupUser
    {
        /// <summary>
        /// Gets the user that belongs to a group.
        /// </summary>
        IContentReference User { get; internal set; }
    }
}