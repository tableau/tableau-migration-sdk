using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class MigrationHookFactoryTests
    {
        #region - Test Types -

        private interface ITestHook : IMigrationHook<int> { }

        private interface IDifferentHook : IMigrationHook<Guid> { }

        private class TestHook : ITestHook
        {
            public Task<int> ExecuteAsync(int ctx, CancellationToken cancel)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region - Create -

        public class Create
        {
            [Fact]
            public void CreatesFromFactory()
            {
                var h = new TestHook();
                var f = new MigrationHookFactory(s => h);

                var result = f.Create<ITestHook>(new Mock<IServiceProvider>().Object);

                Assert.Same(h, result);
            }

            [Fact]
            public void ThrowsOnInvalidHookType()
            {
                var h = new TestHook();
                var f = new MigrationHookFactory(s => h);

                Assert.Throws<InvalidCastException>(() => f.Create<IDifferentHook>(new Mock<IServiceProvider>().Object));
            }
        }

        #endregion
    }
}
