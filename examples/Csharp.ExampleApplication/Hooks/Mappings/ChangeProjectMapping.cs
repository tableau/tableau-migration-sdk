using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;

namespace Csharp.ExampleApplication.Hooks.Mappings
{
    #region class
    public class ChangeProjectMapping : IContentMapping<IDataSource>, IContentMapping<IWorkbook>
    {
        private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

        private readonly ILogger<ChangeProjectMapping> _logger;

        public ChangeProjectMapping(ILogger<ChangeProjectMapping> logger)
        {
            _logger = logger;
        }

        private async Task<ContentMappingContext<T>?> ExecuteAsync<T>(ContentMappingContext<T> ctx)
            where T : IContentReference, IMappableContainerContent
        {
            // Get the container (project) location for the content item.
            var containerLocation = ctx.ContentItem.Location.Parent();

            // We only want to map content items whose project name is "Test".
            if (!StringComparer.Equals("Test", containerLocation.Name))
            {
                return ctx;
            }

            // Build the new project location.
            var newContainerLocation = containerLocation.Rename("Production");

            // Build the new content item location.
            var newLocation = newContainerLocation.Append(ctx.ContentItem.Location.Name);

            // Map the new content item location.
            ctx = ctx.MapTo(newLocation);

            _logger.LogInformation(
                "{ContentType} mapped from {OldLocation} to {NewLocation}.",
                typeof(T).Name,
                ctx.ContentItem.Location,
                ctx.MappedLocation);

            return await ctx.ToTask();
        }

        public async Task<ContentMappingContext<IDataSource>?> ExecuteAsync(ContentMappingContext<IDataSource> ctx, CancellationToken cancel)
            => await ExecuteAsync(ctx);

        public async Task<ContentMappingContext<IWorkbook>?> ExecuteAsync(ContentMappingContext<IWorkbook> ctx, CancellationToken cancel)
            => await ExecuteAsync(ctx);
    }
    #endregion
}
