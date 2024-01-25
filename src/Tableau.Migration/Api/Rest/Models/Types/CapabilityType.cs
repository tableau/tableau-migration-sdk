using System.Xml.Serialization;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// Class that defines the Capability Type.
    /// </summary>
    public class CapabilityType
    {
        internal CapabilityType()
        { }

        /// <summary>
        /// Constructor to build from <see cref="ICapability"/>.
        /// </summary>
        /// <param name="capability"></param>
        public CapabilityType(ICapability capability)
        {
            Name = capability.Name;
            Mode = capability.Mode;
        }
        /// <inheritdoc/>
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <inheritdoc/>
        [XmlAttribute("mode")]
        public string? Mode { get; set; }
    }
}
