using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for an API client sign-in result model. 
    /// </summary>
    internal class SignInResult : ISignInResult
    {
        /// <inheritdoc/>
        public string Token { get; }

        /// <inheritdoc/>
        public Guid SiteId { get; }

        /// <inheritdoc/>
        public string SiteContentUrl { get; }

        /// <inheritdoc/>
        public Guid UserId { get; }

        /// <summary>
        /// Creates a new <see cref="SignInResult"/> instance.
        /// </summary>
        /// <param name="response">The REST API sign-in response.</param>
        public SignInResult(SignInResponse response)
        {
            var credentials = Guard.AgainstNull(response.Item, () => response.Item);
            var site = Guard.AgainstNull(response.Item.Site, () => response.Item.Site);
            var user = Guard.AgainstNull(response.Item.User, () => response.Item.User);

            Token = Guard.AgainstNullEmptyOrWhiteSpace(credentials.Token, () => response.Item.Token);
            SiteId = Guard.AgainstDefaultValue(site.Id, () => response.Item.Site.Id);
            SiteContentUrl = Guard.AgainstNullOrWhiteSpace(site.ContentUrl, () => response.Item.Site.ContentUrl);
            UserId = Guard.AgainstDefaultValue(user.Id, () => response.Item.User.Id);
        }
    }
}
