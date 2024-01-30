using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;

namespace Csharp.ExampleApplication.Hooks.MigrationActionCompleted
{
    #region class
    public class LogMigrationActionsHook : IMigrationActionCompletedHook
    {
        private readonly ILogger<LogMigrationActionsHook> _logger;

        public LogMigrationActionsHook(ILogger<LogMigrationActionsHook> logger)
        {
            _logger = logger;
        }

        public Task<IMigrationActionResult?> ExecuteAsync(IMigrationActionResult ctx, CancellationToken cancel)
        {
            if (ctx.Success)
            {
                _logger.LogInformation("Migration action completed successfully.");
            }
            else
            {
                _logger.LogWarning(
                    "Migration action completed with errors:{NewLine}{Errors}",
                    Environment.NewLine,
                    String.Join(Environment.NewLine, ctx.Errors.Select(e => e.ToString())));
            }

            return Task.FromResult<IMigrationActionResult?>(ctx);
        }
    }
    #endregion
}
