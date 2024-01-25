using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Content.Permissions
{
    /// <inheritdoc/>    
    public class Capability : ICapability
    {
        /// <summary>
        /// Constructor to convert from <see cref="CapabilityType"/>.
        /// </summary>
        /// <param name="response"></param>
        internal Capability(CapabilityType response)
        {
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            Mode = Guard.AgainstNullEmptyOrWhiteSpace(response.Mode, () => response.Mode);
        }

        /// <summary>
        /// Constructor to build from <see cref="Name"/> and <see cref="Mode"/>.
        /// </summary>
        /// <param name="name">The capability name.</param>
        /// <param name="mode">The capability mode from <see cref="PermissionsCapabilityModes"/>.</param>        
        internal Capability(string name, string mode)
        {
            Name = Guard.AgainstNullEmptyOrWhiteSpace(name, () => name);
            Mode = Guard.AgainstNullEmptyOrWhiteSpace(mode, () => mode);
        }
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Mode { get; set; }
    }
}
