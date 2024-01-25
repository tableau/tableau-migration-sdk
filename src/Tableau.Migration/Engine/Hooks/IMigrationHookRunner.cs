using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface for an object that can run hooks.
    /// </summary>
    public interface IMigrationHookRunner
    {
        /// <summary>
        /// Executes all hooks for the hook type in order.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <typeparam name="TContext">The hook context type.</typeparam>
        /// <param name="context">The context to pass to the first hook.</param>
        /// <param name="cancel"></param>
        /// <returns>The result context returned by the last hook.</returns>
        Task<TContext> ExecuteAsync<THook, TContext>(TContext context, CancellationToken cancel)
            where THook : IMigrationHook<TContext>;
    }
}
