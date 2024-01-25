using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a local group creation response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateGroupResponse : TableauServerResponse<CreateGroupResponse.GroupType>
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
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the import information for the response.
            /// </summary>
            [XmlElement("import")]
            public ImportType? Import { get; set; }

            /// <summary>
            /// Class representing an import response.
            /// </summary>
            public class ImportType
            {
                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("domainName")]
                public string? DomainName { get; set; }

                /// <summary>
                /// Gets or sets the site role for the response.
                /// </summary>
                [XmlAttribute("siteRole")]
                public string? SiteRole { get; set; }

                /// <summary>
                /// Gets or sets the grant license mode for the response.
                /// </summary>
                [XmlAttribute("grantLicenseMode")]
                public string? GrantLicenseMode { get; set; }
            }
        }
    }
}
