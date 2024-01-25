namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a data source REST response.
    /// </summary>
    public interface IDataSourceType : IRestIdentifiable, INamedContent, IWithProjectType, IWithOwnerType, IWithTagTypes
    {
        /// <summary>
        /// Gets the description for the response.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the content URL for the response.
        /// </summary>
        string? ContentUrl { get; }

        /// <summary>
        /// Gets the created timestamp for the response.
        /// </summary>
        string? CreatedAt { get; }

        /// <summary>
        /// Gets the updated timestamp for the response.
        /// </summary>
        string? UpdatedAt { get; }

        /// <summary>
        /// Gets the encrypted extracts flag for the response.
        /// </summary>
        bool EncryptExtracts { get; }

        /// <summary>
        /// Gets whether or not the data source has extracts for the response.
        /// </summary>
        bool HasExtracts { get; }

        /// <summary>
        /// Gets whether or not the data source is certified for the response.
        /// </summary>
        bool IsCertified { get; }

        /// <summary>
        /// Gets whether or not the data source uses a remote query agent for the response.
        /// </summary>
        bool UseRemoteQueryAgent { get; }

        /// <summary>
        /// Gets the data source webpage URL for the response.
        /// </summary>
        string? WebpageUrl { get; }
    }
}
