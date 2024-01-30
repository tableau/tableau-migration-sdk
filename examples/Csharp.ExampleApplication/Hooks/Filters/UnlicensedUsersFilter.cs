using System;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Filters
{
    #region class
    public class UnlicensedUsersFilter : ContentFilterBase<IUser>
    {
        public UnlicensedUsersFilter(
            ISharedResourcesLocalizer localizer,
            ILogger<IContentFilter<IUser>> logger)
                : base(localizer, logger) { }

        public override bool ShouldMigrate(ContentMigrationItem<IUser> item)
        {
            return !string.Equals(item.SourceItem.SiteRole, SiteRoles.Unlicensed, StringComparison.OrdinalIgnoreCase);
        }
    }
    #endregion
}
