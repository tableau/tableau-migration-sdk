namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a connection REST response.
    /// </summary>
    public interface IConnectionType : IRestIdentifiable
    {
        /// <summary>
        /// The connection type for the response.
        /// </summary>
        public string? Type { get; }

        /// <summary>
        /// The server address for the response.
        /// </summary>
        public string? ServerAddress { get; }

        /// <summary>
        /// The server port for the response.
        /// </summary>
        public string? ServerPort { get; }

        /// <summary>
        /// The connection username for the response.
        /// </summary>
        public string? ConnectionUsername { get; }

        /// <summary>
        /// The query tagging enabled flag for the response. 
        /// This is returned only for administrator users.
        /// </summary>
        public string? QueryTaggingEnabled { get; }
    }
}
