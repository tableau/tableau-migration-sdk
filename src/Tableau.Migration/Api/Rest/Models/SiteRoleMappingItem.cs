namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// A record class for the mappings of a user's <see cref="SiteRoles"/>, <see cref="AdministratorLevels"/>, <see cref="LicenseLevels"/>
    /// and Publishing Capability (boolean)
    /// </summary>
    public record SiteRoleMappingItem(string SiteRole, string AdministratorLevel, string LicenseLevel, bool CanPublish);
}
