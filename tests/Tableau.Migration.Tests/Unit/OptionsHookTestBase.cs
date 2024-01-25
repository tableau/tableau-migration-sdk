using Moq;
using Tableau.Migration.Engine.Options;

namespace Tableau.Migration.Tests.Unit
{
    public abstract class OptionsHookTestBase<TOptions> : AutoFixtureTestBase
        where TOptions : class, new()
    {
        protected readonly Mock<IMigrationPlanOptionsProvider<TOptions>> MockOptionsProvider;

        protected TOptions Options { get; set; }

        public OptionsHookTestBase()
        {
            Options = new();

            MockOptionsProvider = new();
            MockOptionsProvider.Setup(x => x.Get()).Returns(() => Options);
        }
    }
}
