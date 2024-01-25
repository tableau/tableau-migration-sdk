using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface to represent the response returned by the Add to group method in <see cref="IUsersApiClient"/>.
    /// </summary>
    public interface IAddUserToGroupResult : IRestIdentifiable
    {
        /// <summary>
        /// Gets the Username of the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the SiteRole of the user.
        /// </summary>
        string SiteRole { get; }
    }
}