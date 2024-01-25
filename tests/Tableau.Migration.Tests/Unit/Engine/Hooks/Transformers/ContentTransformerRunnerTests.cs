using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Transformers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers
{
    public class ContentTransformerRunnerTests
    {
        #region - Test Types -

        private class TestTransformer : ContentTransformerBase<TestContentType>
        {
            public override Task<TestContentType?> ExecuteAsync(TestContentType itemToTransform, CancellationToken cancel)
            {
                itemToTransform.ContentUrl += "Transformed";
                return Task.FromResult<TestContentType?>(itemToTransform);
            }
        }

        private class ExceptionTransformer : ContentTransformerBase<TestContentType>
        {
            public override Task<TestContentType?> ExecuteAsync(TestContentType itemToTransform, CancellationToken cancel)
            {
                itemToTransform.ContentUrl += "Transformed";
                throw new Exception("This is a failure!");
            }
        }

        #endregion

        #region - ExecuteAsync -

        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly List<ContentMigrationItem<TestContentType>> _transformerExecutionContexts;
            private readonly List<IMigrationHookFactory> _transformerFactories;

            private readonly IMigrationPlan _plan;

            private readonly ContentTransformerRunner _runner;

            public ExecuteAsync()
            {
                _transformerExecutionContexts = new();
                _transformerFactories = new();

                var mockTransformers = AutoFixture.Create<Mock<IMigrationHookFactoryCollection>>();
                mockTransformers.Setup(x => x.GetHooks<IContentTransformer<TestContentType>>())
                    .Returns(() => _transformerFactories.ToImmutableArray());

                var mockPlan = AutoFixture.Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.Transformers).Returns(mockTransformers.Object);
                _plan = mockPlan.Object;
                Assert.NotNull(_plan.Transformers);
                _runner = new(_plan, new Mock<IServiceProvider>().Object);
            }

            [Fact]
            public async Task SingleTransformerSingleItem()
            {
                // Arrange
                var input = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer()));

                // Act
                var result = await _runner.ExecuteAsync<TestContentType>(input, default);

                // Assert
                Assert.Contains("Transformed", result.ContentUrl);
                Assert.Single(Regex.Matches(result.ContentUrl, "Transformed"));
            }

            [Fact]
            public async Task MultipleTransformerSingleItem()
            {
                // Arrange
                var input = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer()));
                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer()));

                // Act
                var result = await _runner.ExecuteAsync(input, default);

                // Assert
                Assert.Contains("Transformed", result.ContentUrl);
                Assert.Equal(2, Regex.Matches(result.ContentUrl, "Transformed").Count);
            }

            [Fact]
            public async Task SingleTransformerMultipleItem()
            {
                // Arrange
                var input1 = AutoFixture.Create<TestContentType>();
                var input2 = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer()));

                // Act
                var result1 = await _runner.ExecuteAsync(input1, default);
                var result2 = await _runner.ExecuteAsync(input2, default);

                // Assert
                Assert.Contains("Transformed", result1.ContentUrl);
                Assert.Single(Regex.Matches(result1.ContentUrl, "Transformed"));
                Assert.Contains("Transformed", result2.ContentUrl);
                Assert.Single(Regex.Matches(result2.ContentUrl, "Transformed"));
            }

            [Fact]
            public async Task MultpleTransformerMultipleItem()
            {
                // Arrange
                var input1 = AutoFixture.Create<TestContentType>();
                var input2 = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer()));
                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer()));

                // Act
                var result1 = await _runner.ExecuteAsync(input1, default);
                var result2 = await _runner.ExecuteAsync(input2, default);

                // Assert
                Assert.Contains("Transformed", result1.ContentUrl);
                Assert.Equal(2, Regex.Matches(result1.ContentUrl, "Transformed").Count);
                Assert.Contains("Transformed", result2.ContentUrl);
                Assert.Equal(2, Regex.Matches(result1.ContentUrl, "Transformed").Count);
            }

            [Fact]
            public async Task FailureCase()
            {
                // Arrange
                var input = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new ExceptionTransformer()));

                // Act
                //
                // Create an action that runs the _runner, which will throw because the transformation throws
                var act = async () => await _runner.ExecuteAsync(input, default);

                // Assert
                //
                // Verify that the runner throws
                await Assert.ThrowsAsync<Exception>(act);
            }
        }

        #endregion
    }
}
