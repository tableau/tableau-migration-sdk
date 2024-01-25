using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a site response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class SiteResponse : TableauServerResponse<SiteResponse.SiteType>
    {
        /// <summary>
        /// Gets or sets the site for the response.
        /// </summary>
        [XmlElement("site")]
        public override SiteType? Item { get; set; }

        /// <summary>
        /// Class representing a site response.
        /// </summary>
        public class SiteType : IRestIdentifiable, IApiContentUrl
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
            /// Gets or sets the content URL for the response.
            /// </summary>
            [XmlAttribute("contentUrl")]
            public string? ContentUrl { get; set; }
        }
    }
}
