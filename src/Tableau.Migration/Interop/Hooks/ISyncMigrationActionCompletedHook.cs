using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;

namespace Tableau.Migration.Interop.Hooks
{
    /// <summary>
    /// Interface representing a hook called synchronously when a <see cref="IMigrationAction"/> completes.
    /// </summary>
    public interface ISyncMigrationActionCompletedHook
        : ISyncMigrationHook<IMigrationActionResult>, IMigrationActionCompletedHook
    {
        /// <summary>
        /// Executes a migration action completed callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous hook.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new IMigrationActionResult? Execute(IMigrationActionResult ctx);
    }
}
