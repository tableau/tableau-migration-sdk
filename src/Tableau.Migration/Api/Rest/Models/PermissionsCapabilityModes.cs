namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// The capability modes for a specific content type.
    /// </summary>
    public class PermissionsCapabilityModes : StringEnum<PermissionsCapabilityModes>
    {
        /// <summary>
        /// The name of the Allow capability mode.
        /// </summary>
        public const string Allow = "Allow";

        /// <summary>
        /// The name of the Deny capability mode.
        /// </summary>
        public const string Deny = "Deny";
    }
}
