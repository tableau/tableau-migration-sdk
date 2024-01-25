using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal class AddUserToGroupResult : IAddUserToGroupResult
    {
        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string SiteRole { get; }

        public AddUserToGroupResult(AddUserResponse response)
        {
            var user = Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(user.Id, () => response.Item.Id);

            Name = Guard.AgainstNullEmptyOrWhiteSpace(user.Name, () => response.Item.Name);

            SiteRole = Guard.AgainstNullEmptyOrWhiteSpace(user.SiteRole, () => response.Item.SiteRole);
        }
    }
}
