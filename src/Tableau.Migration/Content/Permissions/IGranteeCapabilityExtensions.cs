using System;
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// Extension methods for <see cref="IGranteeCapability"/>.
    /// </summary>
    internal static class IGranteeCapabilityExtensions
    {
        public static GranteeCapabilityType ToGranteeCapabilityType(this IGranteeCapability granteeCapability)
            => new(granteeCapability);

        public static GranteeCapabilityType[] ToGranteeCapabilityTypes(this IEnumerable<IGranteeCapability> granteeCapabilities)
            => granteeCapabilities.Select(ToGranteeCapabilityType).ToArray();


        /// <summary>
        /// Defaults the mode to <see cref="PermissionsCapabilityModes.Deny"/> 
        /// where there is both an <see cref="PermissionsCapabilityModes.Allow"/> and <see cref="PermissionsCapabilityModes.Deny"/> 
        /// <see cref="ICapability.Mode"/> for the same <see cref="ICapability.Name"/>.
        /// </summary>
        /// <param name="uniqueCapabilities"></param>
        /// <returns></returns>
        public static HashSet<ICapability> ResolveCapabilityModeConflicts(this HashSet<ICapability> uniqueCapabilities)
        {

            foreach (var grouping in uniqueCapabilities.GroupBy(c => c.Name).Where(x => x.Count() > 1))
            {
                var allowCapabilities = grouping.Where(
                    item =>
                    PermissionsCapabilityModes.Allow.Equals(item.Mode, StringComparison.OrdinalIgnoreCase));

                var denyCapabilities = grouping.Where(
                    item =>
                    PermissionsCapabilityModes.Deny.Equals(item.Mode, StringComparison.OrdinalIgnoreCase));

                if (allowCapabilities.Any() && denyCapabilities.Any())
                {
                    foreach (var cap in allowCapabilities)
                    {
                        uniqueCapabilities.Remove(cap);
                    }

                    foreach (var cap in denyCapabilities.SkipLast(1))
                    {
                        uniqueCapabilities.Remove(cap);
                    }
                    continue;
                }
            }

            return uniqueCapabilities;
        }

        /// <summary>
        /// Defaults the mode to <see cref="PermissionsCapabilityModes.Deny"/> 
        /// where there is both an <see cref="PermissionsCapabilityModes.Allow"/> and <see cref="PermissionsCapabilityModes.Deny"/> 
        /// <see cref="ICapability.Mode"/> for the same <see cref="ICapability.Name"/>.
        /// </summary>
        /// <param name="capabilities"></param>
        /// <returns></returns>
        public static HashSet<ICapability> ResolveCapabilityModeConflicts(this IEnumerable<ICapability> capabilities)
        {
            return ResolveCapabilityModeConflicts(new HashSet<ICapability>(capabilities, ICapabilityComparer.Instance));
        }
    }
}
