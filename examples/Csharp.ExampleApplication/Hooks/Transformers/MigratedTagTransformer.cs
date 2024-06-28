using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Transformers
{
    #region class
    public class MigratedTagTransformer<T> : ContentTransformerBase<T> where T : IContentReference, IWithTags
    {
        private readonly ILogger<IContentTransformer<T>>? _logger;

        public MigratedTagTransformer(ISharedResourcesLocalizer localizer, ILogger<IContentTransformer<T>> logger) : base(localizer, logger)
        {
            _logger = logger;
        }

        public override async Task<T?> TransformAsync(T itemToTransform, CancellationToken cancel)
        {
            var tag = "Migrated";

            // Add the tag to the content item.
            itemToTransform.Tags.Add(new Tag(tag));

            _logger?.LogInformation(
                @"Added ""{Tag}"" tag to {ContentType} {ContentLocation}.",
                tag,
                typeof(T).Name,
                itemToTransform.Location);

            return await Task.FromResult(itemToTransform);
        }

        public async Task<IPublishableWorkbook?> TransformAsync(IPublishableWorkbook ctx, CancellationToken cancel)
            => await TransformAsync(ctx, cancel);

        public async Task<IPublishableDataSource?> TransformAsync(IPublishableDataSource ctx, CancellationToken cancel)
            => await TransformAsync(ctx, cancel);
    }
    #endregion
}
