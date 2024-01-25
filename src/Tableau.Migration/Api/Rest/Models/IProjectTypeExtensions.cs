using System;

namespace Tableau.Migration.Api.Rest.Models
{
    internal static class IProjectTypeExtensions
    {
        /// <summary>
        /// Gets the parsed parent project ID.
        /// </summary>
        /// <returns>
        /// The parent project ID, 
        /// or null if <see cref="IProjectType.ParentProjectId"/> is null, empty, or fails to parse.
        /// </returns>
        public static Guid? GetParentProjectId(this IProjectType project)
            => ParseProjectId(project.ParentProjectId);

        /// <summary>
        /// Gets the parsed parent project ID.
        /// </summary>
        /// <returns>
        /// The parent project ID, 
        /// or null if <paramref name="projectId"/> is null, empty, or fails to parse.
        /// </returns>
        public static Guid? ParseProjectId(string? projectId)
            => Guid.TryParse(projectId, out var parsedId) ? parsedId : null;

        /// <summary>
        /// Gets the parsed parent project ID.
        /// </summary>
        /// <returns>
        /// The parent project ID, 
        /// or null if <see cref="IProjectType.ControllingPermissionsProjectId"/> is null, empty, or fails to parse.
        /// </returns>
        public static Guid? GetControllingPermissionsProjectId(this IProjectType project)
            => ParseProjectId(project.ControllingPermissionsProjectId);
    }
}
