namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for an object that can be renamed or otherwise
    /// mapped to a new location during migration.
    /// </summary>
    public interface IMappableContent
    {
        /// <summary>
        /// Sets the content location, performing any renames as required.
        /// </summary>
        /// <param name="newLocation">The new location to use.</param>
        void SetLocation(ContentLocation newLocation);
    }
}
