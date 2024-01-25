namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Options for <see cref="TableauCloudUsernameMapping"/>.
    /// </summary>
    public class TableauCloudUsernameMappingOptions
    {
        /// <summary>
        /// Gets the domain to use for generated emails.
        /// </summary>
        public string MailDomain { get; init; } = string.Empty;

        /// <summary>
        /// Gets whether or not existing user emails should be used when available.
        /// </summary>
        public bool UseExistingEmail { get; init; } = true;
    }
}
