using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a user creation response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddUserResponse : TableauServerResponse<AddUserResponse.UserType>
    {
        /// <summary>
        /// The User object.
        /// </summary>
        [XmlElement("user")]
        public override UserType? Item { get; set; }

        /// <summary>
        /// Type for the User object.
        /// </summary>
        public class UserType : IRestIdentifiable
        {
            /// <summary>
            /// The uniquer identifier for the user.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// The Username.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// The site role for the user.
            /// </summary>
            [XmlAttribute("siteRole")]
            public string? SiteRole { get; set; }

            /// <summary>
            /// The site role for the user.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }
        }
    }
}
