using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an add user to group request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#add_user_to_group">Tableau API Reference</see>  for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddUserToGroupRequest : TableauServerRequest
    {
        /// <summary>
        /// Default parameterless constructor.
        /// </summary>
        public AddUserToGroupRequest() { }

        /// <summary>
        /// Creates a new <see cref="AddUserToSiteRequest"/> instance.
        /// </summary>
        /// <param name="id">The username.</param>        
        public AddUserToGroupRequest(Guid id)
        {
            User = new UserType
            {
                Id = id
            };
        }

        /// <summary>
        /// Gets or sets the user for the request.
        /// </summary>
        [XmlElement("user")]
        public UserType? User { get; set; }

        /// <summary>
        /// The user type in the API request body.
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

        }
    }
}
