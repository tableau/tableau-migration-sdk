using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// Interface for an object that can take action during a migration.
    /// </summary>
    public interface IMigrationAction
    {
        /// <summary>
        /// Executes the migration action.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>An awaitable task for the overall action result.</returns>
        Task<IMigrationActionResult> ExecuteAsync(CancellationToken cancel);
    }
}
