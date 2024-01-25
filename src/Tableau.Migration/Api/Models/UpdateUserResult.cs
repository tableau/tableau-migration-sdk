using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal class UpdateUserResult : IUpdateUserResult
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string? FullName { get; }

        /// <inheritdoc/>
        public string? Email { get; }

        /// <inheritdoc/>
        public string SiteRole { get; }

        /// <inheritdoc/>
        public string AuthSetting { get; }

        public UpdateUserResult(UpdateUserResponse response)
        {
            var user = Guard.AgainstNull(response.Item, () => response.Item);

            Name = Guard.AgainstNullEmptyOrWhiteSpace(user.Name, () => response.Item.Name);
            FullName = user.FullName;
            Email = user.Email;
            SiteRole = Guard.AgainstNullEmptyOrWhiteSpace(user.SiteRole, () => response.Item.SiteRole);
            AuthSetting = Guard.AgainstNullEmptyOrWhiteSpace(user.AuthSetting, () => response.Item.AuthSetting);
        }
    }
}
