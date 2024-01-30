using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Filters
{
    #region class
    public class DefaultProjectsFilter : ContentFilterBase<IProject>
    {
        public DefaultProjectsFilter(
            ISharedResourcesLocalizer localizer,
            ILogger<IContentFilter<IProject>> logger) : base(localizer, logger) { }

        public override bool ShouldMigrate(ContentMigrationItem<IProject> item)
        {
            return !string.Equals(item.SourceItem.Name, "default", System.StringComparison.OrdinalIgnoreCase);
        }
    }
    #endregion
}
