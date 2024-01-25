using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a user content item.
    /// </summary>
    public interface IUser : IUsernameContent
    {
        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        string FullName { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the site role of the user.
        /// </summary>
        string SiteRole { get; set; }

        /// <summary>
        /// Gets or sets the authentication type of the user, 
        /// or null to not send an explicit authentication type for the user during migration.
        /// </summary>
        string? AuthenticationType { get; set; }

        /// <summary>
        /// Gets the user's administrator level derived from <see cref="SiteRole"/>.
        /// </summary>
        public string AdministratorLevel => SiteRoleMapping.GetAdministratorLevel(SiteRole);

        /// <summary>
        /// Gets the user's license level derived from <see cref="SiteRole"/>.
        /// </summary>
        public string LicenseLevel => SiteRoleMapping.GetLicenseLevel(SiteRole);

        /// <summary>
        /// Gets the user's publish capability derived from <see cref="SiteRole"/>.
        /// </summary>
        public bool CanPublish => SiteRoleMapping.GetPublishingCapability(SiteRole);
    }
}