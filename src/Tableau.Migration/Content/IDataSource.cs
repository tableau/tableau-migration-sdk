namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a data source content item.
    /// </summary>
    public interface IDataSource :
        IContentReference,
        IPublishedContent,
        IDescriptionContent,
        IExtractContent,
        IWithTags,
        IContainerContent,
        IMappableContainerContent,
        IPermissionsContent,
        IRequiresOwnerUpdate,
        IWithConnections,
        IRequiresLabelUpdate
    {
        /// <summary>
        /// Gets whether or not the data source has extracts.
        /// </summary>
        bool HasExtracts { get; }

        /// <summary>
        /// Gets the IsCertified flag for the data source.
        /// Should be updated through a post-publish hook.
        /// </summary>
        bool IsCertified { get; }

        /// <summary>
        /// Gets or sets the UseRemoteQueryAgent flag for the data source.
        /// </summary>
        bool UseRemoteQueryAgent { get; set; }
    }
}
