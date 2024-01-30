using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Mappings
{
    #region class
    public class ProjectRenameMapping : ContentMappingBase<IProject>
    {
        public ProjectRenameMapping(ISharedResourcesLocalizer localizer, ILogger<IContentMapping<IProject>> logger) : base(localizer, logger)
        { }

        public override async Task<ContentMappingContext<IProject>?> MapAsync(ContentMappingContext<IProject> ctx, CancellationToken cancel)
        {
            if (!String.Equals("Test", ctx.ContentItem.Name, StringComparison.OrdinalIgnoreCase))
            {
                return ctx;
            }

            var newLocation = ctx.ContentItem.Location.Rename("Production");

            ctx = ctx.MapTo(newLocation);

            return await ctx.ToTask();
        }
    }
    #endregion
}
