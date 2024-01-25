namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for an API client project creation model.
    /// </summary>
    public interface ICreateProjectOptions
    {
        /// <summary>
        /// Gets the optional parent project.
        /// </summary>
        IContentReference? ParentProject { get; }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the optional description for the project.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the optional user permissions in a project.
        /// </summary>
        string? ContentPermissions { get; }

        /// <summary>
        /// Gets whether to publish sample workbooks to the project.
        /// </summary>
        bool PublishSamples { get; }
    }
}
