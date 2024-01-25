using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;

namespace MyMigrationApplication.Hooks.Filters
{
    #region class
    public class DefaultProjectsFilter : ContentFilterBase<IProject>
    {
        public override bool ShouldMigrate(ContentMigrationItem<IProject> item)
        {
            return !string.Equals(item.SourceItem.Name, "default", System.StringComparison.OrdinalIgnoreCase);
        }
    }
    #endregion
}
