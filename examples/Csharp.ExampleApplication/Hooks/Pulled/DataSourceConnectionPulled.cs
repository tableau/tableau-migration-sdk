using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Pulled;

namespace Csharp.ExampleApplication.Hooks.Pulled
{
    #region class
    public class DataSourceConnectionPulled : ContentItemPulledHookBase<IPublishableDataSource>
    {
        public override Task<ContentItemPulledContext<IPublishableDataSource>?> ExecuteAsync(ContentItemPulledContext<IPublishableDataSource> ctx, CancellationToken cancel)
        {
            if (ctx.PulledItem.Connections.Any(c => string.Equals(c.Type, "postgres", StringComparison.OrdinalIgnoreCase)))
            {
                ctx.Status = FilterStatus.CascadeSkip;
            }

            return ctx.ToTask();
        }
    }
    #endregion
}
