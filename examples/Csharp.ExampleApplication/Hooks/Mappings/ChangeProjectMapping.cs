using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Mappings
{
    #region class
    public class ChangeProjectMapping<T> : ContentMappingBase<T>
        where T : IContentReference, IMappableContainerContent
    {
        private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

        private readonly ILogger<IContentMapping<T>>? _logger;

        public ChangeProjectMapping(ISharedResourcesLocalizer? localizer, ILogger<IContentMapping<T>>? logger) : base(localizer, logger)
        {
            _logger = logger;
        }

        public override Task<ContentMappingContext<T>?> MapAsync(ContentMappingContext<T> ctx, CancellationToken cancel)
        {
            // Get the container (project) location for the content item.
            var containerLocation = ctx.ContentItem.Location.Parent();

            // We only want to map content items whose project name is "Test".
            if (!StringComparer.Equals("Test", containerLocation.Name))
            {
                return ctx.ToTask();
            }

            // Build the new project location.
            var newContainerLocation = containerLocation.Rename("Production");

            // Build the new content item location.
            var newLocation = newContainerLocation.Append(ctx.ContentItem.Location.Name);

            // Map the new content item location.
            ctx = ctx.MapTo(newLocation);

            _logger?.LogInformation(
                "{ContentType} mapped from {OldLocation} to {NewLocation}.",
                typeof(T).Name,
                ctx.ContentItem.Location,
                ctx.MappedLocation);

            return ctx.ToTask();
        }

        public async Task<ContentMappingContext<IDataSource>?> MapAsync(ContentMappingContext<IDataSource> ctx, CancellationToken cancel)
            => await MapAsync(ctx, cancel);

        public async Task<ContentMappingContext<IWorkbook>?> MapAsync(ContentMappingContext<IWorkbook> ctx, CancellationToken cancel)
            => await MapAsync(ctx, cancel);
    }
    #endregion
}
