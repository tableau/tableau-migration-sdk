using System;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineRunnerTests
    {
        #region - ExecuteAsync -

        public class ExecuteAsync : MigrationPipelineTestBase<TestPipeline>
        {
            private readonly MigrationPipelineRunner _runner;

            public ExecuteAsync()
            {
                _runner = Create<MigrationPipelineRunner>();
            }

            [Fact]
            public async Task RunsAllActionsAsync()
            {
                await _runner.ExecuteAsync(Pipeline, Cancel);

                Assert.All(Pipeline.TestPipelineActions, a => Assert.Equal(1, a.ExecuteCalls));
            }

            [Fact]
            public async Task StopPipelineFromActionResultAsync()
            {
                Pipeline.TestPipelineActions[1].ExecuteResult = MigrationActionResult.Failed(new Exception(), performNextAction: false);

                await _runner.ExecuteAsync(Pipeline, Cancel);

                Assert.Equal(1, Pipeline.TestPipelineActions[0].ExecuteCalls);
                Assert.Equal(1, Pipeline.TestPipelineActions[1].ExecuteCalls);
                Assert.Equal(0, Pipeline.TestPipelineActions[2].ExecuteCalls);
            }

            [Fact]
            public async Task StopPipelineFromHookAsync()
            {
                MockHookRunner.Setup(x => x.ExecuteAsync<IMigrationActionCompletedHook, IMigrationActionResult>(
                    It.Is<IMigrationActionResult>(r => Object.ReferenceEquals(r, Pipeline.TestPipelineActions[1].ExecuteResult)), Cancel))
                    .ReturnsAsync(MigrationActionResult.Succeeded(performNextAction: false));

                await _runner.ExecuteAsync(Pipeline, Cancel);

                Assert.Equal(1, Pipeline.TestPipelineActions[0].ExecuteCalls);
                Assert.Equal(1, Pipeline.TestPipelineActions[1].ExecuteCalls);
                Assert.Equal(0, Pipeline.TestPipelineActions[2].ExecuteCalls);
            }
        }

        #endregion
    }
}
