namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for API client project creation options. 
    /// </summary>
    public class CreateProjectOptions : ICreateProjectOptions
    {
        /// <inheritdoc/>
        public IContentReference? ParentProject { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string? Description { get; }

        /// <inheritdoc/>
        public string? ContentPermissions { get; }

        /// <inheritdoc/>
        public bool PublishSamples { get; }

        /// <summary>
        /// Creates a new <see cref="CreateProjectOptions"/> instance.
        /// </summary>
        /// <param name="parentProject">The parent project if applicable.</param>
        /// <param name="name">The name of the project.</param>
        /// <param name="description">The description of the project.</param>
        /// <param name="contentPermissions">The content permissions for the project.</param>
        /// <param name="publishSamples">True to publish sample content, false otherwise.</param>
        public CreateProjectOptions(
            IContentReference? parentProject,
            string name,
            string? description,
            string? contentPermissions,
            bool publishSamples)
        {
            if (parentProject is not null)
            {
                Guard.AgainstDefaultValue(parentProject.Id, () => parentProject.Id);
                Guard.AgainstDefaultValue(parentProject.Location, () => parentProject.Location);
                ParentProject = parentProject;
            }

            Name = Guard.AgainstNullEmptyOrWhiteSpace(name, nameof(name));
            Description = description;
            ContentPermissions = contentPermissions;
            PublishSamples = publishSamples;
        }
    }
}
