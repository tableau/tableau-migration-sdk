using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Tests.Content.Permissions
{
    internal class IPermissionsComparer : ComparerBase<IPermissions>
    {
        public static readonly IPermissionsComparer Instance = new();

        public override int CompareItems(IPermissions x, IPermissions y)
        {
            var parentIdResult = Comparer<Guid?>.Default.Compare(x.ParentId, y.ParentId);

            if (parentIdResult != 0)
                return parentIdResult;

            return IGranteeCapabilityComparer.Instance.Compare(x.GranteeCapabilities, y.GranteeCapabilities);
        }

        public bool Equals(PermissionsType x, IPermissions y) => Equals(new Migration.Content.Permissions.Permissions(x), y);

        public override int GetHashCode([DisallowNull] IPermissions obj)
            => HashCode.Combine(
                obj.ParentId,
                IGranteeCapabilityComparer.Instance.GetHashCode(obj.GranteeCapabilities));
    }
}
