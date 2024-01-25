using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Excludes users with <see cref="SiteRoles.SupportUser"/> siterole from migration.
    /// </summary>
    public class UserSiteRoleSupportUserFilter : ContentFilterBase<IUser>
    {
        /// <inheritdoc/>
        public override bool ShouldMigrate(ContentMigrationItem<IUser> item)
            => !SiteRoles.IsAMatch(item.SourceItem.SiteRole, SiteRoles.SupportUser);
    }
}
