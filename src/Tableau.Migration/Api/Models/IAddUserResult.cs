using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface to represent the response returned by the Add method in <see cref="IUsersApiClient"/>.
    /// </summary>
    public interface IAddUserResult : IRestIdentifiable
    {
        /// <summary>
        /// Gets the Username of the user.
        /// </summary>
        string Name { get; }

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