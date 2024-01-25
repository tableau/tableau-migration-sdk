namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a group content item.
    /// </summary>
    public interface IGroup : IUsernameContent
    {
        /// <summary>
        /// Gets the grant license mode of the group.
        /// </summary>
        string? GrantLicenseMode { get; set; }

        /// <summary>
        /// Gets the site role of the group.
        /// </summary>
        string? SiteRole { get; set; }
    }
}