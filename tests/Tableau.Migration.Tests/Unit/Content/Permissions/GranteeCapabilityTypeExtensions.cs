using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Tests.Unit.Content.Permissions
{
    internal static class GranteeCapabilityTypeExtensions
    {
        public static IGranteeCapability ToIGranteeCapability(this GranteeCapabilityType granteeCapability)
            => new GranteeCapability(granteeCapability);

        public static IImmutableList<IGranteeCapability> ToIGranteeCapabilities(this IEnumerable<GranteeCapabilityType> granteeCapabilities)
            => granteeCapabilities.Select(c => c.ToIGranteeCapability()).ToImmutableArray();
    }
}
