using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that can migration Tableau data between Tableau sites.
    /// </summary>
    public interface IMigrator
    {
        /// <summary>
        /// Executes a migration asynchronously.
        /// </summary>
        /// <param name="plan">The migration plan to execute.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The results of the migration.</returns>
        Task<MigrationResult> ExecuteAsync(IMigrationPlan plan, CancellationToken cancel);

        /// <summary>
        /// Executes a migration asynchronously.
        /// </summary>
        /// <param name="plan">The migration plan to execute.</param>
        /// <param name="previousManifest">A manifest from a previous migration of the same plan to use to determine what progress has already been made.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The results of the migration.</returns>
        Task<MigrationResult> ExecuteAsync(IMigrationPlan plan, IMigrationManifest? previousManifest, CancellationToken cancel);
    }
}
