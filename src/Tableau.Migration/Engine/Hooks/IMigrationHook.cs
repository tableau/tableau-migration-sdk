using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface for a callback that is called by the migration engine.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    public interface IMigrationHook<TContext>
    {
        /// <summary>
        /// Executes a hook callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous hook.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>
        /// A task to await containing the context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        Task<TContext?> ExecuteAsync(TContext ctx, CancellationToken cancel);
    }
}