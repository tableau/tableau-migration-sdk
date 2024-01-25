using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Default <see cref="IMigrationPipelineRunner"/> implementation.
    /// </summary>
    public class MigrationPipelineRunner : IMigrationPipelineRunner
    {
        private readonly IMigrationHookRunner _hooks;

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineRunner"/> object.
        /// </summary>
        /// <param name="hooks">The hook runner.</param>
        public MigrationPipelineRunner(IMigrationHookRunner hooks)
        {
            _hooks = hooks;
        }

        /// <inheritdoc />
        public async Task<IResult> ExecuteAsync(IMigrationPipeline pipeline, CancellationToken cancel)
        {
            var resultBuilder = new ResultBuilder();

            foreach (var action in pipeline.BuildActions())
            {
                var actionResult = await action.ExecuteAsync(cancel).ConfigureAwait(false);

                actionResult = await _hooks.ExecuteAsync<IMigrationActionCompletedHook, IMigrationActionResult>(actionResult, cancel).ConfigureAwait(false);
                resultBuilder.Add(actionResult);

                //Exit pipeline early if requested by the action or a hook.
                if (actionResult.PerformNextAction == false)
                {
                    return actionResult;
                }
            }

            return resultBuilder.Build();
        }
    }
}
