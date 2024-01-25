using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// The type definition for content item.
    /// </summary>
    public class PermissionsContentItemType : IRestIdentifiable
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

}
