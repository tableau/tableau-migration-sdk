namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a class representing a server's version information.
    /// </summary>
    public interface ITableauServerVersionProvider
    {
        /// <summary>
        /// Gets the current Tableau server's version information.
        /// </summary>
        TableauServerVersion? Version { get; }

        /// <summary>
        /// Sets the current version information.
        /// </summary>
        /// <param name="version">The server's version information.</param>
        void Set(TableauServerVersion version);
    }
}