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
    public class SharedCustomViewFilter : ContentFilterBase<ICustomView>
    {
        public SharedCustomViewFilter(
            ISharedResourcesLocalizer localizer,
            ILogger<IContentFilter<ICustomView>> logger)
                : base(localizer, logger) { }

        public override bool ShouldMigrate(ContentMigrationItem<ICustomView> item)
        {
            return !item.SourceItem.Shared;
        }
    }
    #endregion
}
