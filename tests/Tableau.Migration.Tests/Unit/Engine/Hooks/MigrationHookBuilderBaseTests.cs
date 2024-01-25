using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class MigrationHookBuilderBaseTests
    {
        public class TestMigrationHookBuilderBase : MigrationHookBuilderBase
        {
            public void PublicAddFactoriesByType(Type hookType, Func<IServiceProvider, object> initializer)
                => AddFactoriesByType(hookType, initializer);

            public IEnumerable<KeyValuePair<Type, IEnumerable<IMigrationHookFactory>>> PublicGetFactories()
                => GetFactories();
        }

        public interface ITestHook : IMigrationHook<TestContentType>
        { }

        public interface ITestPublishHook : IMigrationHook<TestPublishType>
        { }

        public class MigrationHookBuilderBaseTest : AutoFixtureTestBase
        {
            protected readonly TestMigrationHookBuilderBase Builder;

            public MigrationHookBuilderBaseTest()
            {
                Builder = new();
            }
        }

        public class GetFactories : MigrationHookBuilderBaseTest
        {
            [Fact]
            public void GetsAllFactories()
            {
                var type1 = typeof(ITestHook);
                var fac1 = (IServiceProvider s) => Create<ITestHook>();
                var fac2 = (IServiceProvider s) => Create<ITestHook>();

                var type2 = typeof(ITestPublishHook);
                var fac3 = (IServiceProvider s) => Create<ITestPublishHook>();

                Builder.PublicAddFactoriesByType(type1, fac1);
                Builder.PublicAddFactoriesByType(type1, fac2);
                Builder.PublicAddFactoriesByType(type2, fac3);

                var result = Builder.PublicGetFactories()
                    .ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());

                Assert.Equal(2, result.Count);

                Assert.True(result.TryGetValue(type1, out var resultFactories));
                Assert.Equal(2, resultFactories.Length);

                Assert.True(result.TryGetValue(type2, out resultFactories));
                Assert.Single(resultFactories);
            }
        }
    }
}
