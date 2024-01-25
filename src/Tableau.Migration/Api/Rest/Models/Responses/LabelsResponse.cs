using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a local group creation response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class LabelsResponse : TableauServerListResponse<LabelsResponse.LabelType>
    {
        /// <summary>
        /// Gets or sets the labels.
        /// </summary>        
        [XmlArray("labelList")]
        [XmlArrayItem("label")]
        public override LabelType[] Items { get; set; } = Array.Empty<LabelType>();

        /// <summary>
        /// Class representing the label in the response.
        /// </summary>        
        public class LabelType
        {
            /// <summary>
            /// Gets or sets the ID.
            /// </summary>
            [XmlElement("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the site.
            /// </summary>
            [XmlElement("site")]
            public SiteType? Site { get; set; }

            /// <summary>
            /// Gets or sets the owner.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            /// <summary>
            /// Gets or sets the user display name.
            /// </summary>
            [XmlAttribute("userDisplayName")]
            public string? UserDisplayName { get; set; }

            /// <summary>
            /// Gets or sets the ID for the label's content item.
            /// </summary>
            [XmlAttribute("contentId")]
            public Guid ContentId { get; set; }

            /// <summary>
            /// Gets or sets the type for the label's content item.
            /// </summary>
            [XmlAttribute("contentType")]
            public string? ContentType { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            [XmlAttribute("message")]
            public string? Message { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            [XmlAttribute("value")]
            public string? Value { get; set; }

            /// <summary>
            /// Gets or sets the category.
            /// </summary>
            [XmlAttribute("category")]
            public string? Category { get; set; }

            /// <summary>
            /// Gets or sets the active flag.
            /// </summary>
            [XmlAttribute("active")]
            public bool Active { get; set; }

            /// <summary>
            /// Gets or sets the active flag.
            /// </summary>
            [XmlAttribute("elevated")]
            public bool Elevated { get; set; }

            /// <summary>
            /// Gets or sets the create timestamp.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the update timestamp.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Class representing the site in the response.
            /// </summary>
            [XmlRoot("site")]
            public class SiteType
            {
                /// <summary>
                /// The ID for the site.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Class representing the owner in the response.
            /// </summary>
            [XmlRoot("owner")]
            public class OwnerType : IRestIdentifiable
            {
                /// <summary>
                /// The ID for the owner.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

        }
    }
}
