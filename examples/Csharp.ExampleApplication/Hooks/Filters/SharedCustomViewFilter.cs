using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
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

        public override void Filter(ContentFilterContextItem<ICustomView> item)
        {
            if (item.SourceItem.Shared)
            {
                item.Status = FilterStatus.Skip;
            }
        }
    }
    #endregion
}
