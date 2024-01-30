using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Hooks.PostPublish;

namespace Csharp.ExampleApplication.Hooks.PostPublish
{
    #region class
    public class BulkLoggingHook<T> : BulkPostPublishHookBase<T>
    {
        private readonly ILogger<BulkLoggingHook<T>> _logger;

        public BulkLoggingHook(ILogger<BulkLoggingHook<T>> logger)
        {
            _logger = logger;
        }

        public override Task<BulkPostPublishContext<T>?> ExecuteAsync(BulkPostPublishContext<T> ctx, CancellationToken cancel)
        {
            // Log the number of items published in the batch.
            _logger.LogInformation(
                "Published {Count} {ContentType} item(s).",
                ctx.PublishedItems.Count,
                typeof(T).Name);

            return Task.FromResult<BulkPostPublishContext<T>?>(ctx);
        }
    }
    #endregion
}
