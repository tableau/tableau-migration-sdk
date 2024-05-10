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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public class ContentFilterRunnerTests
    {
        #region - Test Types -
        private interface ITestMigrationItems : IEnumerable<ContentMigrationItem<IUser>>
        { }

        private class TestMigrationItems : IEnumerable<ContentMigrationItem<IUser>>
        {
            private readonly List<ContentMigrationItem<IUser>> migrationItems = new();

            public IEnumerator<ContentMigrationItem<IUser>> GetEnumerator()
            {
                foreach (var entry in migrationItems)
                {
                    yield return entry;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class TestFilter : IContentFilter<IUser>
        {
            private readonly List<IEnumerable<ContentMigrationItem<IUser>>> _contexts;
            private readonly IEnumerable<ContentMigrationItem<IUser>>? _result;


            public TestFilter(List<IEnumerable<ContentMigrationItem<IUser>>> contexts,
                TestMigrationItems? result)
            {
                _contexts = contexts;
                _result = result;
            }

            public Task<IEnumerable<ContentMigrationItem<IUser>>?> ExecuteAsync(IEnumerable<ContentMigrationItem<IUser>> ctx, CancellationToken cancel)
            {
                _contexts.Add(ctx);
                return Task.FromResult(_result);
            }
        }

        #endregion

        #region - ExecuteAsync -

        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly List<IEnumerable<ContentMigrationItem<IUser>>> _filterExecutionContexts;
            private readonly List<IMigrationHookFactory> _filterFactories;

            private readonly IMigrationPlan _plan;

            private readonly ContentFilterRunner _runner;

            public ExecuteAsync()
            {
                _filterExecutionContexts = new();
                _filterFactories = new();

                var mockFilters = AutoFixture.Create<Mock<IMigrationHookFactoryCollection>>();
                mockFilters.Setup(x => x.GetHooks<IContentFilter<IUser>>()).Returns(() => _filterFactories.ToImmutableArray());

                var mockPlan = AutoFixture.Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.Filters).Returns(mockFilters.Object);
                _plan = mockPlan.Object;
                Assert.NotNull(_plan.Filters);
                _runner = new(_plan, new Mock<IServiceProvider>().Object);
            }

            private void AddFilterWithResult(TestMigrationItems? result)
            {
                var filter = new TestFilter(_filterExecutionContexts, result);
                _filterFactories.Add(new MigrationHookFactory(s => filter));
            }

            [Fact]
            public async Task AllowsOrderedContextOverwriteAsync()
            {
                // Arrange
                //
                // Setup 3 different results for the 3 different filters. 
                // The filters don't do anything other then return the result it was given (for testing purposes)
                var result1 = new TestMigrationItems();
                var result2 = new TestMigrationItems();
                var result3 = new TestMigrationItems();

                AddFilterWithResult(result1);
                AddFilterWithResult(result2);
                AddFilterWithResult(result3);

                // Setup the input context that will be used to verify the filters
                var input = new TestMigrationItems();

                // Act
                var result = await _runner.ExecuteAsync(input, default);

                // Assert
                //
                // Verify that the output of the plan, is the result of the last filter we set
                Assert.Same(result3, result);

                // TestFilter.ExecuteAsync receives the output of the previous filter as the input. 
                // TestFilter has a reference to _filterExecutionContext (passed in via ctor) so the
                // input context order is saved back to _transformerExecutionContexts because TestFilter saves them
                // just to verify that the order is correct
                Assert.Equal(new[] { input, result1, result2 }, _filterExecutionContexts);
            }

            [Fact]
            public async Task NullResultReturnsInputAsync()
            {
                // Arrange
                //
                // Create 3 filters that do not modify the input context
                // and hence the input context is used as the result
                // See IMigrationHook.ExecuteContext
                AddFilterWithResult(null);
                AddFilterWithResult(null);
                AddFilterWithResult(null);

                var input = new TestMigrationItems();

                // Act
                var result = await _runner.ExecuteAsync(input, default);

                // Assert
                //
                // Verify that that input context is the same as the result after all filters
                // as no filters updated the input context
                Assert.Same(input, result);

                // TestFilter.ExecuteAsync receives the output of the previous filter as the input. 
                // TestFilter has a reference to _filterExecutionContext (passed in via ctor) so the
                // input context order is saved back to _transformerExecutionContexts because TestFilter saves them
                // just to verify that the order is correct
                // In this case, none of the filters updated the input context, so they should all be the same
                Assert.Equal(new[] { input, input, input }, _filterExecutionContexts);
            }
        }

        #endregion
    }
}
