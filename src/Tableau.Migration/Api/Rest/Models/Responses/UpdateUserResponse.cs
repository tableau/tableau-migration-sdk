using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a user update response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateUserResponse : TableauServerResponse<UpdateUserResponse.UserType>
    {
        /// <summary>
        /// The User object.
        /// </summary>
        [XmlElement("user")]
        public override UserType? Item { get; set; }

        /// <summary>
        /// Type for the User object.
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// The new Username of the user.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// The new full-name of the user.
            /// </summary>
            [XmlAttribute("fullName")]
            public string? FullName { get; set; }

            /// <summary>
            /// The new email address for the user.
            /// </summary>
            [XmlAttribute("email")]
            public string? Email { get; set; }

            /// <summary>
            /// The new site role for the user.
            /// </summary>
            [XmlAttribute("siteRole")]
            public string? SiteRole { get; set; }

            /// <summary>
            /// The new auth setting for the user.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }
        }
    }
}
