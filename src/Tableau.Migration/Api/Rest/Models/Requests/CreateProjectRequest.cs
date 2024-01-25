using System;
using System.Xml.Serialization;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing a project creation request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_projects.htm#create_project">Tableau API Reference</see> for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateProjectRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the project for the request.
        /// </summary>
        [XmlElement("project")]
        public ProjectType? Project { get; set; }

        /// <summary>
        /// Creates a new <see cref="CreateProjectRequest"/> instance.
        /// </summary>
        public CreateProjectRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CreateProjectRequest"/> instance.
        /// </summary>
        /// <param name="parentProjectId">The project's parent project ID, if applicable.</param>
        /// <param name="name">The project name.</param>
        /// <param name="description">The project description.</param>
        /// <param name="contentPermissions">The project's content permissions.</param>
        public CreateProjectRequest(Guid? parentProjectId, string name, string? description, string? contentPermissions)
        {
            Project = new ProjectType
            {
                ParentProjectId = parentProjectId?.ToString(),
                Name = name,
                Description = description,
                ContentPermissions = contentPermissions
            };
        }

        /// <summary>
        /// Creates a new <see cref="CreateProjectRequest"/> instance.
        /// </summary>
        /// <param name="options">The project creation options.</param>
        public CreateProjectRequest(ICreateProjectOptions options)
            : this(options.ParentProject?.Id, options.Name, options.Description, options.ContentPermissions)
        { }

        /// <summary>
        /// Class representing a project request.
        /// </summary>
        public class ProjectType
        {
            /// <summary>
            /// Gets or sets the parent project ID for the request.
            /// </summary>
            [XmlAttribute("parentProjectId")]
            public string? ParentProjectId { get; set; }

            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the description for the request.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the content permissions for the request.
            /// </summary>
            [XmlAttribute("contentPermissions")]
            public string? ContentPermissions { get; set; }

            /// <summary>
            /// Gets the parsed parent project ID.
            /// </summary>
            public Guid? GetParentProjectId() => IProjectTypeExtensions.ParseProjectId(ParentProjectId);
        }
    }
}
