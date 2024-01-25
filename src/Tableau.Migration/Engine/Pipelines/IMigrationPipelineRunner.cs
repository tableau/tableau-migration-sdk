using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Interface for an object that can execute <see cref="IMigrationPipeline"/>s.
    /// </summary>
    public interface IMigrationPipelineRunner
    {
        /// <summary>
        /// Executes all pipeline actions.
        /// </summary>
        /// <param name="pipeline">The pipeline to execute.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>An awaitable task for the overall pipeline execution result.</returns>
        Task<IResult> ExecuteAsync(IMigrationPipeline pipeline, CancellationToken cancel);
    }
}
