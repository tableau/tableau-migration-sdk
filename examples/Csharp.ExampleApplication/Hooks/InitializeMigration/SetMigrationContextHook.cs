using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Engine.Hooks.InitializeMigration;

namespace Csharp.ExampleApplication.Hooks.InitializeMigration
{
    #region class

    internal class SetMigrationContextHook : IInitializeMigrationHook
    {
        public Task<IInitializeMigrationHookResult?> ExecuteAsync(IInitializeMigrationHookResult ctx, CancellationToken cancel)
        {
            var customContext = ctx.ScopedServices.GetRequiredService<CustomContext>();
            customContext.CustomerId = Guid.NewGuid();

            return Task.FromResult<IInitializeMigrationHookResult?>(ctx);
        }
    }

    #endregion
}
