using System;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api.Rest
{
    internal class PermissionsUriBuilder : ContentItemUriBuilderBase, IPermissionsUriBuilder
    {
        public PermissionsUriBuilder(string prefix, string suffix = "permissions")
            : base(prefix, suffix)
        { }

        public virtual string BuildDeleteUri(
            Guid contentItemId,
            ICapability capability,
            GranteeType granteeType,
            Guid granteeId)
        {
            Guard.AgainstDefaultValue(contentItemId, nameof(contentItemId));
            Guard.AgainstDefaultValue(granteeId, nameof(granteeId));
            Guard.AgainstNullEmptyOrWhiteSpace(capability.Name, nameof(capability.Name));
            Guard.AgainstNullEmptyOrWhiteSpace(capability.Mode, nameof(capability.Mode));

            return $"{BuildUri(contentItemId)}/{granteeType.ToUrlSegment()}/{granteeId.ToUrlSegment()}/{capability.Name}/{capability.Mode}";
        }
    }
}
