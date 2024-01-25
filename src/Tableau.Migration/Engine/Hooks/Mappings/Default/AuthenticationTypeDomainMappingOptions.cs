namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Options for <see cref="AuthenticationTypeDomainMapping"/>.
    /// </summary>
    public class AuthenticationTypeDomainMappingOptions
    {
        /// <summary>
        /// Gets the domain to map user domains to.
        /// </summary>
        public string UserDomain { get; init; } = string.Empty;

        /// <summary>
        /// Gets the domain to map group domains to.
        /// </summary>
        public string GroupDomain { get; init; } = string.Empty;
    }
}
