using System;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for API client data source publish options. 
    /// </summary>
    public interface IPublishDataSourceOptions : IPublishFileOptions
    {
        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the data source.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets whether or not the data source uses Tableau Bridge.
        /// </summary>
        bool UseRemoteQueryAgent { get; }

        ///<inheritdoc/>
        bool EncryptExtracts { get; }

        /// <summary>
        /// Gets whether or not to overwrite any existing data source.
        /// </summary>
        bool Overwrite { get; }

        /// <summary>
        /// Gets the ID of the project to publish to.
        /// </summary>
        Guid ProjectId { get; }
    }
}
