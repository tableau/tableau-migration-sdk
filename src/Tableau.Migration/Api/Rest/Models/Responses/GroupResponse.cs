using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a group response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class GroupResponse : TableauServerResponse<GroupResponse.GroupType>
    {
        /// <summary>
        /// Gets or sets the group for the response.
        /// </summary>
        [XmlElement("group")]
        public override GroupType? Item { get; set; }

        /// <summary>
        /// Class representing a group response.
        /// </summary>
        public class GroupType : IRestIdentifiable
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
            public string? Name { get; }

            /// <summary>
            /// Gets or sets the domain for the response.
            /// </summary>
            [XmlElement("domain")]
            public DomainType? Domain { get; set; }

            /// <summary>
            /// Class representing a domain response.
            /// </summary>
            public class DomainType
            {
                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }
        }
    }
}
