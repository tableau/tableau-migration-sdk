using System;
using System.Threading;
using Moq;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineTestBase<TPipeline> : AutoFixtureTestBase
    {
        protected readonly Mock<IServiceProvider> MockServices;
        protected readonly Mock<IMigrationHookRunner> MockHookRunner;

        protected readonly TPipeline Pipeline;

        public MigrationPipelineTestBase()
        {
            MockServices = Freeze<MockServiceProvider>();
            MockServices.Setup(x => x.GetService(typeof(TestAction))).Returns(() => new TestAction());

            MockHookRunner = Freeze<Mock<IMigrationHookRunner>>();
            MockHookRunner.Setup(x => x.ExecuteAsync<IMigrationActionCompletedHook, IMigrationActionResult>(It.IsAny<IMigrationActionResult>(), Cancel))
                .ReturnsAsync((IMigrationActionResult r, CancellationToken c) => r);

            Pipeline = Create<TPipeline>();
        }
    }
}
