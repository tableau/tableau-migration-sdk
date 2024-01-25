namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// The license levels to be dreived from a siterole in <see cref="SiteRoles"/>.
    /// </summary>
    public class LicenseLevels : StringEnum<LicenseLevels>
    {
        /// <summary>
        /// Gets the name of the Creator license level.
        /// </summary>
        public const string Creator = "Creator";

        /// <summary>
        /// Gets the name of the Explorer license level.
        /// </summary>
        public const string Explorer = "Explorer";

        /// <summary>
        /// Gets the name of the Viewer license level.
        /// </summary>
        public const string Viewer = "Viewer";

        /// <summary>
        /// Gets the name of the license level when a user is unlicensed.
        /// </summary>
        public const string Unlicensed = "Unlicensed";
    }
}
