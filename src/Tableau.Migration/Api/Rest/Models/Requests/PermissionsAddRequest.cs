using System.Xml;
using System.Xml.Serialization;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// The base class for all Add Permissions Response classes.    
    /// </summary>
    [XmlType(XmlTypeName)]
    public class PermissionsAddRequest : TableauServerRequest
    {
        /// <summary>
        /// Default parameterless constructor.
        /// </summary>
        protected PermissionsAddRequest()
        { }

        /// <summary>
        /// Constructor to build from <see cref="IPermissions"/>.
        /// </summary>
        /// <param name="permissions"></param>
        public PermissionsAddRequest(IPermissions permissions)
        {
            Permissions.GranteeCapabilities = permissions.GranteeCapabilities.ToGranteeCapabilityTypes();
        }

        /// <summary>
        /// The permissions XML element.
        /// </summary>
        [XmlElement("permissions")]
        public PermissionsType? Permissions { get; set; } = new();
    }

}
