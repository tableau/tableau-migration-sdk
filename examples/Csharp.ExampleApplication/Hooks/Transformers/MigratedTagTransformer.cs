using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Csharp.ExampleApplication.Hooks.Transformers
{
    #region class
    public class MigratedTagTransformer : IContentTransformer<IPublishableWorkbook>, IContentTransformer<IPublishableDataSource>
    {
        private readonly ILogger<MigratedTagTransformer> _logger;

        public MigratedTagTransformer(ILogger<MigratedTagTransformer> logger)
        {
            _logger = logger;
        }

        protected async Task<T?> ExecuteAsync<T>(T ctx)
            where T : IContentReference, IWithTags
        {
            var tag = "Migrated";

            // Add the tag to the content item.
            ctx.Tags.Add(new Tag(tag));

            _logger.LogInformation(
                @"Added ""{Tag}"" tag to {ContentType} {ContentLocation}.",
                tag,
                typeof(T).Name,
                ctx.Location);

            return await Task.FromResult(ctx);
        }

        public async Task<IPublishableWorkbook?> ExecuteAsync(IPublishableWorkbook ctx, CancellationToken cancel)
            => await ExecuteAsync(ctx);

        public async Task<IPublishableDataSource?> ExecuteAsync(IPublishableDataSource ctx, CancellationToken cancel)
            => await ExecuteAsync(ctx);
    }
    #endregion
}
