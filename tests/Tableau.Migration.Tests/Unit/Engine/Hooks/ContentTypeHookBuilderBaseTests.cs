using System;
using System.Collections.Immutable;
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class ContentTypeHookBuilderBaseTests
    {
        public class TestContentTypeHookBuilderBase
            : ContentTypeHookBuilderBase
        {
            public void PublicAddFactoriesByType(Type hookType, Func<IServiceProvider, object> initializer)
                => AddFactoriesByType(hookType, initializer);
        }

        public interface ITestHook<T> : IMigrationHook<T>
        { }

        public class ContentTypeHookBuilderBaseTest : AutoFixtureTestBase
        {
            protected readonly TestContentTypeHookBuilderBase Builder = new();
        }

        public class ByContentType : ContentTypeHookBuilderBaseTest
        {
            [Fact]
            public void ReturnsFactoriesByContentType()
            {
                var type1 = typeof(ITestHook<TestContentType>);
                var fac1 = (IServiceProvider s) => Create<ITestHook<TestContentType>>();
                var fac2 = (IServiceProvider s) => Create<ITestHook<TestContentType>>();

                var type2 = typeof(ITestHook<TestPublishType>);
                var fac3 = (IServiceProvider s) => Create<ITestHook<TestPublishType>>();

                Builder.PublicAddFactoriesByType(type1, fac1);
                Builder.PublicAddFactoriesByType(type1, fac2);
                Builder.PublicAddFactoriesByType(type2, fac3);

                var result = Builder.ByContentType()
                    .ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());

                Assert.Equal(2, result.Count);

                Assert.True(result.TryGetValue(typeof(TestContentType), out var resultFactories));
                Assert.Equal(2, resultFactories.Length);

                Assert.True(result.TryGetValue(typeof(TestPublishType), out resultFactories));
                Assert.Single(resultFactories);
            }
        }
    }
}
