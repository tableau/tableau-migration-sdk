using System;
using System.ComponentModel.DataAnnotations;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Object that describes how to connect to a <see cref="IApiClient"/>.
    /// </summary>
    /// <param name="ServerUrl">The base URL of the Tableau Server, or the Tableau Cloud URL to connect to.</param>
    /// <param name="SiteContentUrl">The content URL of the site to connect to. Can be empty string for default site.</param>
    /// <param name="AccessTokenName">The name of the personal access token to use to sign into the site.</param>
    /// <param name="AccessToken">The personal access token to use to sign into the site.</param>
    public readonly record struct TableauSiteConnectionConfiguration(
        Uri ServerUrl,
        string SiteContentUrl,
        [property: Required] string AccessTokenName,
        [property: Required] string AccessToken
    )
    {
        /// <summary>
        /// A <see cref="TableauSiteConnectionConfiguration"/> with empty values, useful to detect if a value has not yet been configured without using null.
        /// </summary>
        public static readonly TableauSiteConnectionConfiguration Empty = new(new Uri("https://localhost"), string.Empty, string.Empty, string.Empty);

        /// <summary>
        /// Validates that the connection configuration has enough information to connect.
        /// </summary>
        /// <returns>The validation result.</returns>
        public IResult Validate() => this.ValidateSimpleProperties();
    }
}
