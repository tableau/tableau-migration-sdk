namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a <see cref="IDataSource"/> that has been downloaded
    /// and has full information necessary for re-publishing.
    /// </summary>
    public interface IPublishableDataSource : IDataSource, IFileContent, IConnectionsContent
    {
        /// <summary>
        /// Gets the certification note for the data source.
        /// Should be updated through a post-publish hook.
        /// </summary>
        string CertificationNote { get; }
    }
}
