using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Migrators.Batch;

namespace Csharp.ExampleApplication.Hooks.BatchMigrationCompleted
{
    #region class
    public class LogMigrationBatchesHook<T> : IContentBatchMigrationCompletedHook<T>
        where T : IContentReference
    {
        private readonly ILogger<LogMigrationBatchesHook<T>> _logger;

        public LogMigrationBatchesHook(ILogger<LogMigrationBatchesHook<T>> logger)
        {
            _logger = logger;
        }

        public Task<IContentBatchMigrationResult<T>?> ExecuteAsync(IContentBatchMigrationResult<T> ctx, CancellationToken cancel)
        {
            _logger.LogInformation(
                "{ContentType} batch of {Count} item(s) completed:{NewLine}{Statuses}",
                typeof(T).Name,
                ctx.ItemResults.Count,
                Environment.NewLine,
                String.Join(Environment.NewLine, ctx.ItemResults.Select(r => $"{r.ManifestEntry.Source.Location}: {r.ManifestEntry.Status}")));

            return Task.FromResult<IContentBatchMigrationResult<T>?>(ctx);
        }
    }
    #endregion
}
