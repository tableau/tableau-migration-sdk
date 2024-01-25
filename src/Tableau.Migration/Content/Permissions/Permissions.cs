using System;
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Permissions
{
    internal class Permissions : IPermissions
    {
        /// <inheritdoc/>
        public IGranteeCapability[] GranteeCapabilities { get; set; } = Array.Empty<IGranteeCapability>();

        /// <inheritdoc/>
        public Guid? ParentId { get; set; }

        public Permissions()
        { }

        public Permissions(PermissionsResponse response)
            : this(response.ParentId, response.Item?.GranteeCapabilities?.Select(c => new GranteeCapability(c)))
        { }

        public Permissions(PermissionsType permissions)
            : this(permissions.ContentItem?.Id, permissions.GranteeCapabilities?.Select(c => new GranteeCapability(c)))
        { }

        public Permissions(IPermissions permissions)
            : this(permissions.ParentId, permissions.GranteeCapabilities)
        { }

        public Permissions(Guid? parentId, IEnumerable<IGranteeCapability>? granteeCapabilities = null)
            : this()
        {
            ParentId = parentId;

            if (granteeCapabilities != null)
            {
                GranteeCapabilities = granteeCapabilities.ToArray();
            }
        }
    }
}