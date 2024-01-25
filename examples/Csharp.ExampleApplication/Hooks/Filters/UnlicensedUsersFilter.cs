using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;

namespace MyMigrationApplication.Hooks.Filters
{
    #region class
    public class UnlicensedUsersFilter : ContentFilterBase<IUser>
    {
        public override bool ShouldMigrate(ContentMigrationItem<IUser> item)
        {
            return !string.Equals(item.SourceItem.SiteRole, SiteRoles.Unlicensed, System.StringComparison.OrdinalIgnoreCase);
        }
    }
    #endregion
}
