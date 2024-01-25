namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// <para>
    /// Class containing authentication type constants.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#add_user_to_site">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class AuthenticationTypes : StringEnum<AuthenticationTypes>
    {
        /// <summary>
        /// Gets the name of the server default authentication type.
        /// </summary>
        public const string ServerDefault = "ServerDefault";

        /// <summary>
        /// Gets the name of the Open ID authentication type.
        /// </summary>
        public const string OpenId = "OpenID";

        /// <summary>
        /// Gets the name of the SAML authentication type.
        /// </summary>
        public const string Saml = "SAML";

        /// <summary>
        /// Gets the name of the Tableau ID with MFA authentication type.
        /// </summary>
        public const string TableauIdWithMfa = "TableauIDWithMFA";
    }
}
