namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for content type-specific API clients
    /// </summary>
    public interface IContentApiClient
    {
        /// <summary>
        /// Gets the REST URL content type prefix for the client.
        /// </summary>
        string UrlPrefix { get; }
    }
}