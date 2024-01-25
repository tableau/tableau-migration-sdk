using System;
using System.Xml;
using System.Xml.Serialization;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// The base class for all Permissions Response classes.    
    /// </summary>   
    [XmlType(XmlTypeName)]
    public class PermissionsResponse
        : TableauServerWithParentResponse<PermissionsType>
    {
        /// <summary>
        /// The permissions XML element.
        /// </summary>
        [XmlElement("permissions")]
        public override PermissionsType? Item { get; set; }

        /// <summary>
        /// The Id of the parent content type.
        /// </summary>
        [XmlIgnore]
        public Guid? ParentId => Parent?.Id;
    }

}
