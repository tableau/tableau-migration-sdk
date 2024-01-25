using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal class Group : UsernameContentBase, IGroup
    {
        public string? GrantLicenseMode { get; set; }

        public string? SiteRole { get; set; }

        public Group(GroupsResponse.GroupType response)
        {
            var domain = Guard.AgainstNull(response.Domain, () => response.Domain);

            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);

            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            Domain = Guard.AgainstNullEmptyOrWhiteSpace(domain.Name, () => response.Domain.Name);

            GrantLicenseMode = response?.Import?.GrantLicenseMode;
            SiteRole = response?.Import?.SiteRole;
        }

        public Group(CreateGroupResponse response)
        {
            var group = Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(group.Id, () => response.Item.Id);

            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.Name, () => response.Item.Name);

            if (response.Item.Import is null)
            {
                Domain = Constants.LocalDomain;
            }
            else
            {
                Domain = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.Import.DomainName, () => response.Item.Import.DomainName);
            }
        }

        protected Group(IGroup group)
        {
            Id = group.Id;
            Name = group.Name;
            Domain = group.Domain;
            GrantLicenseMode = group.GrantLicenseMode;
            SiteRole = group.SiteRole;
        }
    }
}
