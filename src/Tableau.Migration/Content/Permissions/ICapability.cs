using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// The interface for a grantee's capability.
    /// </summary>
    public interface ICapability
    {
        /// <summary>
        /// The capability name from <see cref="PermissionsCapabilityNames"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The capability mode from <see cref="PermissionsCapabilityModes"/>
        /// </summary>
        public string Mode { get; }
    }
}