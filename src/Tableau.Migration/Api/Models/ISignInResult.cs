using System;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for an API client sign-in result model. 
    /// </summary>
    public interface ISignInResult
    {
        /// <summary>
        /// Gets the authentication token for the sign-in.
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Gets the ID for the signed-in site.
        /// </summary>
        Guid SiteId { get; }

        /// <summary>
        /// Gets the content URL for the signed-in site.
        /// </summary>
        string SiteContentUrl { get; }

        /// <summary>
        /// Gets the ID for the signed-in user.
        /// </summary>
        Guid UserId { get; }
    }
}