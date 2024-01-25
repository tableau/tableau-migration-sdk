using System;
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Permissions
{
    /// <inheritdoc/>
    public class GranteeCapability : IGranteeCapability
    {
        /// <summary>
        /// Constructor to convert from <see cref="GranteeCapabilityType"/>.
        /// </summary>
        /// <param name="response"></param>
        internal GranteeCapability(GranteeCapabilityType response)
            : this(response.GranteeType, response.GranteeId, response.Capabilities.Select(c => new Capability(c)))
        { }

        internal GranteeCapability(
            GranteeType granteeType,
            Guid granteeId,
            IEnumerable<ICapability> capabilities)
        {
            GranteeType = granteeType;
            GranteeId = granteeId;

            Capabilities = new HashSet<ICapability>(capabilities, ICapabilityComparer.Instance);
        }

        /// <inheritdoc/>
        public virtual void ResolveCapabilityModeConflicts()
            => Capabilities.ResolveCapabilityModeConflicts();

        /// <inheritdoc/>
        public GranteeType GranteeType { get; set; }

        /// <inheritdoc/>
        public Guid GranteeId { get; set; }

        /// <inheritdoc/>
        public HashSet<ICapability> Capabilities { get; set; }
    }
}
