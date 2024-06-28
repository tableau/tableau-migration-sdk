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
    public class EncryptExtractsTransformer<T> : ContentTransformerBase<T> where T : IContentReference, IFileContent, IExtractContent
    {
        private readonly ILogger<IContentTransformer<T>>? _logger;

        public EncryptExtractsTransformer(ISharedResourcesLocalizer localizer, ILogger<IContentTransformer<T>> logger) : base(localizer, logger)
        {
            _logger = logger;
        }

        public override async Task<T?> TransformAsync(T itemToTransform, CancellationToken cancel)
        {
            itemToTransform.EncryptExtracts = true;

            _logger?.LogInformation(
                @"Setting encrypt extract to true for {ContentType} {ContentLocation}",
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
