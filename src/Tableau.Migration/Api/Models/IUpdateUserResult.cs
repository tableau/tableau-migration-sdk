namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface to represent the response returned by the Update method in <see cref="IUsersApiClient"/>.
    /// </summary>
    public interface IUpdateUserResult
    {
        /// <summary>
        /// Gets the Name of the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        string? FullName { get; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Gets the SiteRole of the user.
        /// </summary>
        string SiteRole { get; }

        /// <summary>
        /// The AuthSetting for the user.
        /// </summary>
        string AuthSetting { get; }
    }
}