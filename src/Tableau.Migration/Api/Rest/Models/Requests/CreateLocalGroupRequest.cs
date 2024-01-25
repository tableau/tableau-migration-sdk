using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing a local group creation request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#create_group">Tableau API Reference</see> for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateLocalGroupRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the group for the request.
        /// </summary>
        [XmlElement("group")]
        public GroupType? Group { get; set; }

        /// <summary>
        /// Creates a new <see cref="CreateLocalGroupRequest"/> instance.
        /// </summary>
        public CreateLocalGroupRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CreateLocalGroupRequest"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="minimumSiteRole">The minimum site role.</param>
        public CreateLocalGroupRequest(string name, string? minimumSiteRole)
        {
            Group = new GroupType
            {
                Name = name,
                MinimumSiteRole = minimumSiteRole
            };
        }

        /// <summary>
        /// Class representing a group request.
        /// </summary>
        public class GroupType
        {
            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the minimum site role for the request.
            /// </summary>
            [XmlAttribute("minimumSiteRole")]
            public string? MinimumSiteRole { get; set; }
        }
    }
}
