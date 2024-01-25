using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item's embedded connection.
    /// </summary>
    public interface IConnection : IRestIdentifiable
    {
        /// <summary>
        /// Gets the connection type for the response.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the server address for the response.
        /// </summary>
        string? ServerAddress { get; }

        /// <summary>
        /// Gets the server port for the response.
        /// </summary>
        string? ServerPort { get; }

        /// <summary>
        /// Gets the connection username for the response.
        /// </summary>
        string? ConnectionUsername { get; }

        /// <summary>
        /// Gets the query tagging enabled flag for the response. 
        /// This is returned only for administrator users.
        /// </summary>
        bool? QueryTaggingEnabled { get; }
    }
}
