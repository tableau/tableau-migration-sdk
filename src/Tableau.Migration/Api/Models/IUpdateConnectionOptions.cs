namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for update connection options. 
    /// Any combination if these properties are valid. 
    /// If none of the options are provided, no update is made.
    /// </summary>
    public interface IUpdateConnectionOptions
    {
        /// <summary>
        /// The ServerAddress.
        /// </summary>
        public string? ServerAddress { get; }

        /// <summary>
        /// The server port on the connection.
        /// </summary>
        string? ServerPort { get; }

        /// <summary>
        /// The user name on the connection.
        /// </summary>
        public string? ConnectionUsername { get; }

        /// <summary>
        /// The connection password.
        /// </summary>
        string? Password { get; }

        /// <summary>
        /// The embed password flag on the connection.
        /// </summary>
        public bool? EmbedPassword { get; }

        /// <summary>
        /// The query tagging enabled flag on the connection.
        /// </summary>
        public bool? QueryTaggingEnabled { get; }
    }
}
