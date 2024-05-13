//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers
{
    public class ContentTransformerRunnerTests
    {
        #region - Test Types -

        public class TestTransformer : ContentTransformerBase<TestContentType>
        {
            public TestTransformer(
                ISharedResourcesLocalizer localizer, 
                ILogger<TestTransformer> logger) 
                    : base(localizer, logger) { }


            public override Task<TestContentType?> TransformAsync(TestContentType itemToTransform, CancellationToken cancel)
            {
                itemToTransform.ContentUrl += "Transformed";
                return Task.FromResult<TestContentType?>(itemToTransform);
            }
        }

        public class ExceptionTransformer : ContentTransformerBase<TestContentType>
        {
            public ExceptionTransformer(
                ISharedResourcesLocalizer localizer,
                ILogger<ExceptionTransformer> logger) 
                    : base(localizer, logger) { }

            public override Task<TestContentType?> TransformAsync(TestContentType itemToTransform, CancellationToken cancel)
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

            private readonly MockSharedResourcesLocalizer _mockLocalizer = new();
            private readonly Mock<ILogger<TestTransformer>> _mockLogger = new();

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

                _mockLogger.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);
            }

            [Fact]
            public async Task SingleTransformerSingleItem()
            {
                // Arrange
                var input = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer(_mockLocalizer.Object, _mockLogger.Object)));

                // Act
                var result = await _runner.ExecuteAsync<TestContentType>(input, default);

                // Assert
                Assert.Contains("Transformed", result.ContentUrl);
                Assert.Single(Regex.Matches(result.ContentUrl, "Transformed"));

                // Verify we got at least 1 debug log message
                _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
            }

            [Fact]
            public async Task MultipleTransformerSingleItem()
            {
                // Arrange
                var input = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer(_mockLocalizer.Object, _mockLogger.Object)));
                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer(_mockLocalizer.Object, _mockLogger.Object)));

                // Act
                var result = await _runner.ExecuteAsync(input, default);

                // Assert
                Assert.Contains("Transformed", result.ContentUrl);
                Assert.Equal(2, Regex.Matches(result.ContentUrl, "Transformed").Count);

                // Verify we got at least 1 debug log message
                _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
            }

            [Fact]
            public async Task SingleTransformerMultipleItem()
            {
                // Arrange
                var input1 = AutoFixture.Create<TestContentType>();
                var input2 = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer(_mockLocalizer.Object, _mockLogger.Object)));

                // Act
                var result1 = await _runner.ExecuteAsync(input1, default);
                var result2 = await _runner.ExecuteAsync(input2, default);

                // Assert
                Assert.Contains("Transformed", result1.ContentUrl);
                Assert.Single(Regex.Matches(result1.ContentUrl, "Transformed"));
                Assert.Contains("Transformed", result2.ContentUrl);
                Assert.Single(Regex.Matches(result2.ContentUrl, "Transformed"));

                // Verify we got at least 1 debug log message
                _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
            }

            [Fact]
            public async Task MultipleTransformerMultipleItem()
            {
                // Arrange
                var input1 = AutoFixture.Create<TestContentType>();
                var input2 = AutoFixture.Create<TestContentType>();

                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer(_mockLocalizer.Object, _mockLogger.Object)));
                _transformerFactories.Add(new MigrationHookFactory(s => new TestTransformer(_mockLocalizer.Object, _mockLogger.Object)));

                // Act
                var result1 = await _runner.ExecuteAsync(input1, default);
                var result2 = await _runner.ExecuteAsync(input2, default);

                // Assert
                Assert.Contains("Transformed", result1.ContentUrl);
                Assert.Equal(2, Regex.Matches(result1.ContentUrl, "Transformed").Count);
                Assert.Contains("Transformed", result2.ContentUrl);
                Assert.Equal(2, Regex.Matches(result1.ContentUrl, "Transformed").Count);

                // Verify we got at least 1 debug log message
                _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
            }

            [Fact]
            public async Task FailureCase()
            {
                // Arrange
                var input = AutoFixture.Create<TestContentType>();
                var mockLocalizer = new MockSharedResourcesLocalizer();
                var mockExceptionTransformerLogger = new Mock<ILogger<ExceptionTransformer>>();

                _transformerFactories.Add(new MigrationHookFactory(s => new ExceptionTransformer(_mockLocalizer.Object, mockExceptionTransformerLogger.Object)));

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
