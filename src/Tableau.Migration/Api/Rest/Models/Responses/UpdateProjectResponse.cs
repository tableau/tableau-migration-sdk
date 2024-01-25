using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a project update response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateProjectResponse : TableauServerResponse<UpdateProjectResponse.ProjectType>
    {
        /// <summary>
        /// Gets or sets the project object.
        /// </summary>
        [XmlElement("project")]
        public override ProjectType? Item { get; set; }

        /// <summary>
        /// Type for the project object.
        /// </summary>
        public class ProjectType : IRestIdentifiable, IProjectType
        {
            /// <summary>
            /// Gets or sets the id for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the name for the response.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the description for the response.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the parentProjectId for the response.
            /// </summary>
            /// <remarks>
            /// Does not parse due to .NET limitations with nullable XML deserialization.
            /// Use <see cref="IProjectTypeExtensions.GetParentProjectId"/> to get a parsed value.
            /// </remarks>
            [XmlAttribute("parentProjectId")]
            public string? ParentProjectId { get; set; }

            /// <summary>
            /// Gets or sets the content permissions mode for the response.
            /// </summary>
            [XmlAttribute("contentPermissions")]
            public string? ContentPermissions { get; set; }

            /// <summary>
            /// Gets or sets the controllingPermissionsProjectId for the response.
            /// </summary>
            /// <remarks>
            /// Does not parse due to .NET limitations with nullable XML deserialization.
            /// Use <see cref="IProjectTypeExtensions.GetControllingPermissionsProjectId"/> to get a parsed value.
            /// </remarks>
            [XmlAttribute("controllingPermissionsProjectId")]
            public string? ControllingPermissionsProjectId { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            ///<inheritdoc/>
            IOwnerType? IWithOwnerType.Owner => Owner;

            #region - Object Specific Types -

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class OwnerType : IOwnerType
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            #endregion
        }
    }
}
