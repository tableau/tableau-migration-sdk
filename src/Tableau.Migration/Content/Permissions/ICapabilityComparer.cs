using System;
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Content.Permissions
{
    internal class ICapabilityComparer : ComparerBase<ICapability>
    {
        public static readonly ICapabilityComparer Instance = new();

        public override int CompareItems(ICapability x, ICapability y)
        {
            var nameResult = StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name);

            if (nameResult != 0)
                return nameResult;

            return StringComparer.OrdinalIgnoreCase.Compare(x.Mode, y.Mode);
        }

        public override int GetHashCode([DisallowNull] ICapability obj)
            => HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name),
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Mode));
    }
}
