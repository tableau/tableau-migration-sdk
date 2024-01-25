using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal class AddUserResult : IAddUserResult
    {
        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string SiteRole { get; }

        /// <inheritdoc/>
        public string AuthSetting { get; }

        public AddUserResult(AddUserResponse response)
        {
            var user = Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(user.Id, () => response.Item.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(user.Name, () => response.Item.Name);
            SiteRole = Guard.AgainstNullEmptyOrWhiteSpace(user.SiteRole, () => response.Item.SiteRole);
            AuthSetting = Guard.AgainstNullEmptyOrWhiteSpace(user.AuthSetting, () => response.Item.AuthSetting);
        }
    }
}
