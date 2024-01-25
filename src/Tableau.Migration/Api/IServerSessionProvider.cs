using System;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a class representing the current session details for a signed-in user.
    /// </summary>
    public interface IServerSessionProvider
    {
        /// <summary>
        /// Gets the server's version information.
        /// </summary>
        TableauServerVersion? Version { get; }

        /// <summary>
        /// Gets the current site's content URL.
        /// </summary>
        string? SiteContentUrl { get; }

        /// <summary>
        /// Gets the current site's ID.
        /// </summary>
        Guid? SiteId { get; }

        /// <summary>
        /// Gets the current user's ID.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Sets the current user and site information.
        /// </summary>
        /// <param name="signInResult">The sign-in result containing the current user and site information.</param>
        void SetCurrentUserAndSite(ISignInResult signInResult);

        /// <summary>
        /// Sets the current user and site information.
        /// </summary>
        /// <param name="userId">The current user's ID.</param>
        /// <param name="siteId">The current site's ID.</param>
        /// <param name="siteContentUrl">The current site's content URL.</param>
        /// <param name="authenticationToken">The current user's authentication token.</param>
        void SetCurrentUserAndSite(Guid userId, Guid siteId, string siteContentUrl, string authenticationToken);

        /// <summary>
        /// Clears the current user and site information.
        /// </summary>
        void ClearCurrentUserAndSite();

        /// <summary>
        /// Sets the current version information.
        /// </summary>
        /// <param name="version">The server's version information.</param>
        void SetVersion(TableauServerVersion version);
    }
}