using System;
using System.Xml;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// The base class for permissions type classes.
    /// </summary>
    public class PermissionsType
    {
        /// <summary>
        /// The content item.
        /// </summary>
        [XmlIgnore]
        // Project Permissions does not require a content item in the request.
        // Workbook, View and Data Source do. This XmlIgnore is present to establish a pattern.
        public PermissionsContentItemType? ContentItem { get; set; }

        /// <summary>
        /// A collection of Grantee Capabilities.
        /// </summary>
        [XmlElement("granteeCapabilities")]
        public GranteeCapabilityType[]? GranteeCapabilities { get; set; } = Array.Empty<GranteeCapabilityType>();
    }

}
