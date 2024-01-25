namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a project REST response.
    /// </summary>
    public interface IProjectType : IRestIdentifiable, INamedContent, IWithOwnerType
    {
        /// <summary>
        /// Gets the content permissions for the project.
        /// </summary>
        string? ContentPermissions { get; }

        /// <summary>
        /// Gets the description for the project.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the parent project ID for the project.
        /// </summary>
        string? ParentProjectId { get; }

        /// <summary>
        /// Gets the controlling permissions project ID for the project.
        /// </summary>
        string? ControllingPermissionsProjectId { get; }
    }
}