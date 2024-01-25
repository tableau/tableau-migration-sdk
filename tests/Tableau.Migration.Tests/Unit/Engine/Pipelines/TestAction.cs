using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Actions;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class TestAction : IMigrationAction
    {
        public int ExecuteCalls { get; private set; }

        public IMigrationActionResult ExecuteResult { get; set; } = MigrationActionResult.Succeeded();

        public Task<IMigrationActionResult> ExecuteAsync(CancellationToken cancel)
        {
            ExecuteCalls++;
            return Task.FromResult(ExecuteResult);
        }
    }
}
