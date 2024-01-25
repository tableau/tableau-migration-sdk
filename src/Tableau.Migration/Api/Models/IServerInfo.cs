namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for an API client server information model.
    /// </summary>
    public interface IServerInfo
    {
        /// <summary>
        /// Gets the REST API version of the server.
        /// </summary>
        string RestApiVersion { get; }

        /// <summary>
        /// Gets the product version of the server.
        /// </summary>
        string ProductVersion { get; }

        /// <summary>
        /// Gets the build version of the server.
        /// </summary>
        string BuildVersion { get; }

        /// <summary>
        /// Gets a <see cref="TableauServerVersion"/> instance from the version values.
        /// </summary>
        TableauServerVersion TableauServerVersion { get; }
    }
}
