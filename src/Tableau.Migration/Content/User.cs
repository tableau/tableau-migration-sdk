using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal sealed class User : UsernameContentBase, IUser
    {
        /// <inheritdoc/>
        public string FullName { get; set; }

        /// <inheritdoc/>
        public string Email { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string SiteRole { get; set; }

        /// <inheritdoc/>
        public string? AuthenticationType { get; set; }

        public User(UsersResponse.UserType response)
        {
            var domain = Guard.AgainstNull(response.Domain, () => response.Domain);

            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);

            Email = response.Email ?? string.Empty;
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            FullName = Guard.AgainstNullEmptyOrWhiteSpace(response.FullName, () => response.FullName);
            SiteRole = Guard.AgainstNullEmptyOrWhiteSpace(response.SiteRole, () => response.SiteRole);
            AuthenticationType = response.AuthSetting;
            Domain = Guard.AgainstNullEmptyOrWhiteSpace(domain.Name, () => response.Domain.Name);
        }
    }
}
