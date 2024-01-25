using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a flow response.
    /// 
    /// This is incomplete, there are more attributes than we're saving
    /// </summary>
    [XmlType(XmlTypeName)]
    public class FlowsResponse : PagedTableauServerResponse<FlowsResponse.FlowType>
    {
        /// <summary>
        /// Gets or sets the groups for the response.
        /// </summary>
        [XmlArray("flows")]
        [XmlArrayItem("flow")]
        public override FlowType[] Items { get; set; } = Array.Empty<FlowType>();

        /// <summary>
        /// Class representing a site response.
        /// </summary>
        public class FlowType
        {
            /// <summary>
            /// Gets or sets the ID for the response.
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
            /// Gets or sets the webpage URL for the response.
            /// </summary>
            [XmlAttribute("webpageUrl")]
            public string? WebpageUrl { get; set; }

            /// <summary>
            /// Gets or sets the created timestamp for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the updated timestamp for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the project for the response.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public UserType? Owner { get; set; }

            /// <summary>
            /// Gets or sets the tags for the response.
            /// </summary>
            [XmlArray("tags")]
            [XmlArrayItem("tag")]
            public TagType[] Tags { get; set; } = Array.Empty<TagType>();

            /// <summary>
            /// Class representing a REST API project response.
            /// </summary>
            public class ProjectType
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class UserType
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }

            /// <summary>
            /// Class representing a REST API tag response.
            /// </summary>
            public class TagType
            {
                /// <summary>
                /// Gets or sets the label for the response.
                /// </summary>
                [XmlAttribute("label")]
                public string? Label { get; set; }
            }
        }
    }
}
