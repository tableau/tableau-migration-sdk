using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Hooks;

namespace Tableau.Migration.Interop.Hooks
{
    /// <summary>
    /// Interface for a synchronously callback that is called by the migration engine.
    /// </summary>
    /// <typeparam name="TContext"><inheritdoc/></typeparam>
    public interface ISyncMigrationHook<TContext>
        : IMigrationHook<TContext>
    {
        /// <summary>
        /// Executes a hook callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous hook.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        TContext? Execute(TContext ctx);

        /// <inheritdoc />
        Task<TContext?> IMigrationHook<TContext>.ExecuteAsync(TContext ctx, CancellationToken cancel)
            => Task.FromResult(
                Execute(ctx));
    }
}
