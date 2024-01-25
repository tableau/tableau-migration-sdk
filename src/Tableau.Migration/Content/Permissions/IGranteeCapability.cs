using System;
using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// Interface for the grantee of permissions.
    /// </summary>
    public interface IGranteeCapability
    {
        /// <summary>
        /// Indicates the type of grantee.
        /// </summary>
        GranteeType GranteeType { get; }

        /// <summary>
        /// The Id for the User or Group grantee.
        /// </summary>
        Guid GranteeId { get; set; }

        /// <summary>
        /// The collection of capabilities of the grantee.
        /// </summary>
        HashSet<ICapability> Capabilities { get; }

        /// <summary>
        /// Resolves <see cref="ICapability.Mode"/>s where there is both 
        /// an <see cref="PermissionsCapabilityModes.Allow"/> and a <see cref="PermissionsCapabilityModes.Deny"/> 
        /// <see cref="ICapability.Mode"/> for the same <see cref="ICapability.Name"/>.
        /// The default implementation <see cref="IGranteeCapabilityExtensions.ResolveCapabilityModeConflicts(HashSet{ICapability})"/> 
        /// chooses <see cref="PermissionsCapabilityModes.Deny"/> in case of conflict.        
        /// </summary>
        /// <returns></returns>
        void ResolveCapabilityModeConflicts();
    }
}