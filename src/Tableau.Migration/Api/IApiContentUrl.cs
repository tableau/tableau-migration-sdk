namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an API request/response that has a nullable content URL.
    /// </summary>
    public interface IApiContentUrl
    {
        /// <summary>
        /// Gets the content URL.
        /// </summary>
        string? ContentUrl { get; }
    }
}
