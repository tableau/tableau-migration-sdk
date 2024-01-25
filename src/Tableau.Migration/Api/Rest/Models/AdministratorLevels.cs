namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// The Administrator level for the user to be derived from a siterole in <see cref="SiteRoles"/>.
    /// </summary>
    public class AdministratorLevels : StringEnum<AdministratorLevels>
    {
        /// <summary>
        /// Name for the level the level when a user has Site administrator permissions..
        /// </summary>
        public const string Site = "Site";

        /// <summary>
        /// Name for the level the level when a user has no administrator permissions.
        /// </summary>
        public const string None = "None";
    }
}
