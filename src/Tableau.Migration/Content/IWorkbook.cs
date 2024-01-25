namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a workbook content item.
    /// </summary>
    public interface IWorkbook :
        IContentReference,
        IPublishedContent,
        IDescriptionContent,
        IExtractContent,
        IWithTags,
        IContainerContent,
        IMappableContainerContent,
        IPermissionsContent,
        IRequiresOwnerUpdate,
        IWithConnections
    {
        /// <summary>
        /// Gets or sets whether tabs are shown.
        /// </summary>
        bool ShowTabs { get; set; }


        /// <summary>
        /// Gets the file size.
        /// </summary>
        long Size { get; }
    }
}