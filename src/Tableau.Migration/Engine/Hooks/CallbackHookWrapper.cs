using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks
{
    internal class CallbackHookWrapper<THook, TContext> : IMigrationHook<TContext>
        where THook : IMigrationHook<TContext>
    {
        private readonly Func<TContext, CancellationToken, Task<TContext?>> _callback;

        public CallbackHookWrapper(Func<TContext, CancellationToken, Task<TContext?>> callback)
        {
            _callback = callback;
        }

        public async Task<TContext?> ExecuteAsync(TContext ctx, CancellationToken cancel) => await _callback(ctx, cancel).ConfigureAwait(false);
    }
}
