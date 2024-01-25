using System;
using System.Diagnostics.CodeAnalysis;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Tests.Content.Permissions
{
    internal class IGranteeCapabilityComparer : ComparerBase<IGranteeCapability>
    {
        public static IGranteeCapabilityComparer Instance = new();

        private readonly bool _compareGranteeIds;

        public IGranteeCapabilityComparer(bool compareGranteeIds = true) => _compareGranteeIds = compareGranteeIds;

        public override int CompareItems(IGranteeCapability x, IGranteeCapability y)
        {
            var granteeIdResult = _compareGranteeIds ? x.GranteeId.CompareTo(y.GranteeId) : 0;

            if (granteeIdResult != 0)
                return granteeIdResult;

            var granteeTypeResult = x.GranteeType.CompareTo(y.GranteeType);

            if (granteeTypeResult != 0)
                return granteeTypeResult;

            return ICapabilityComparer.Instance.Compare(x.Capabilities, y.Capabilities);
        }

        public override int GetHashCode([DisallowNull] IGranteeCapability obj)
            => HashCode.Combine(_compareGranteeIds ? obj.GranteeId : Guid.Empty, obj.GranteeType, ICapabilityComparer.Instance.GetHashCode(obj.Capabilities));
    }
}
