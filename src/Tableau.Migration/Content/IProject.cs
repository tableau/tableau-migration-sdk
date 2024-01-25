namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a project content item.
    /// </summary>
    public interface IProject :
        IContentReference,
        IDescriptionContent,
        IMappableContainerContent,
        IPermissionsContent,
        IRequiresOwnerUpdate
    {
        /// <summary>
        /// Gets or sets the content permission mode of the project.
        /// </summary>
        string ContentPermissions { get; set; }

        /// <summary>
        /// Gets the parent project reference, 
        /// or null if the project is a top-level project.
        /// Should be changed through mapping.
        /// </summary>
        IContentReference? ParentProject { get; }
    }
}
