//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public class ContentFilterRunnerTests
    {
        #region - Test Types -

        private class TestFilter : IContentFilter<IUser>
        {
            private readonly Action<ContentFilterContextItem<IUser>>? _itemFilter;
            private readonly Func<ContentFilterContext<IUser>, ContentFilterContext<IUser>?>? _contextFilter;

            public TestFilter(Action<ContentFilterContextItem<IUser>>? itemFilter = null,
                Func<ContentFilterContext<IUser>, ContentFilterContext<IUser>?>? contextFilter = null)
            {
                _itemFilter = itemFilter;
                _contextFilter = contextFilter;
            }

            public Task<ContentFilterContext<IUser>?> ExecuteAsync(ContentFilterContext<IUser> ctx, CancellationToken cancel)
            {
                foreach(var item in ctx.Items)
                {
                    _itemFilter?.Invoke(item);
                }

                var resultCtx = _contextFilter?.Invoke(ctx) ?? ctx;
                return Task.FromResult<ContentFilterContext<IUser>?>(resultCtx);
            }
        }

        #endregion

        #region - ExecuteAsync -

        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly List<IMigrationHookFactory> _filterFactories;
            private readonly IMigrationManifestEntryBuilder _entryBuilder;

            private readonly IMigrationPlan _plan;

            private readonly Mock<ILogger<ContentFilterRunner>> _mockLogger;

            private readonly ContentFilterRunner _runner;

            public ExecuteAsync()
            {
                _filterFactories = new();
                _entryBuilder = Freeze<IMigrationManifestEntryBuilder>();

                _mockLogger = Create<Mock<ILogger<ContentFilterRunner>>>();

                var mockFilters = AutoFixture.Create<Mock<IMigrationHookFactoryCollection>>();
                mockFilters.Setup(x => x.GetHooks<IContentFilter<IUser>>()).Returns(() => _filterFactories.ToImmutableArray());

                var mockPlan = AutoFixture.Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.Filters).Returns(mockFilters.Object);
                _plan = mockPlan.Object;
                Assert.NotNull(_plan.Filters);

                _runner = new(_plan, new Mock<IServiceProvider>().Object, Create<ISharedResourcesLocalizer>(), _mockLogger.Object);
            }

            private void AddFilter(Action<ContentFilterContextItem<IUser>>? itemFilter = null,
                Func<ContentFilterContext<IUser>, ContentFilterContext<IUser>?>? contextFilter = null)
            {
                var f = new TestFilter(itemFilter, contextFilter);
                _filterFactories.Add(new MigrationHookFactory(s => f));
            }

            private ContentMigrationItem<T> CreateTestMigrationItem<T>(T item)
                where T : IContentReference
            {
                var manifestEntry = new MigrationManifestEntry(_entryBuilder, new ContentReferenceStub(item));
                return new(item, manifestEntry);
            }

            private IEnumerable<ContentMigrationItem<T>> CreateTestMigrationItems<T>()
                where T : IContentReference
            {
                var items = CreateMany<T>();
                return items.Select(i => CreateTestMigrationItem<T>(i));
            }

            [Fact]
            public async Task AllowsOrderedContextOverwriteAsync()
            {
                // Arrange
                //
                // Setup 3 different results for the 3 different filters. 
                // The filters don't do anything other then return the result it was given (for testing purposes)
                var result1 = new ContentFilterContext<IUser>(CreateTestMigrationItems<IUser>());
                var result2 = new ContentFilterContext<IUser>(CreateTestMigrationItems<IUser>());
                result2.Items[0].Status = FilterStatus.Skip;
                var result3 = new ContentFilterContext<IUser>(CreateTestMigrationItems<IUser>());

                var calledContexts = new List<ContentFilterContext<IUser>>();

                AddFilter(contextFilter: ctx => { calledContexts.Add(ctx); return result1; });
                AddFilter(contextFilter: ctx => { calledContexts.Add(ctx); return result2; });
                AddFilter(contextFilter: ctx => { calledContexts.Add(ctx); return result3; });

                // Setup the input context that will be used to verify the filters
                var input = CreateMany<ContentMigrationItem<IUser>>().ToImmutableArray();

                // Act
                var result = await _runner.ExecuteAsync(input, Cancel);

                // Assert
                //
                // Verify that the output of the plan, is the result of the last filter we set
                Assert.Equal(result3.Items.Where(i => i.ManifestEntry.Status is not MigrationManifestEntryStatus.Skipped).ToImmutableArray(), result);

                // Filters receives the output of the previous filter as the input. 
                // Verify that the order is correct.
                Assert.Equal(3, calledContexts.Count);
                Assert.Equal(input, calledContexts[0].Items);
                Assert.Same(result1, calledContexts[1]);
                Assert.Same(result2, calledContexts[2]);

                // Verify logging was called at least once
                _mockLogger.VerifyLogging(LogLevel.Debug, Times.AtLeastOnce());
            }

            [Fact]
            public async Task NullResultReturnsInputAsync()
            {
                var calledContexts = new List<ContentFilterContext<IUser>>();

                // Arrange
                //
                // Create 3 filters that do not modify the input context
                // and hence the input context is used as the result
                // See IMigrationHook.ExecuteContext
                AddFilter(contextFilter: ctx => { calledContexts.Add(ctx); return null; });
                AddFilter(contextFilter: ctx => { calledContexts.Add(ctx); return null; });
                AddFilter(contextFilter: ctx => { calledContexts.Add(ctx); return null; });

                var input = CreateMany<ContentMigrationItem<IUser>>().ToImmutableArray();

                // Act
                var result = await _runner.ExecuteAsync(input, Cancel);

                // Assert                

                // Filters receives the output of the previous filter as the input. 
                // Verify that the order is correct
                // In this case, none of the filters updated the input context, so they should all be the same.
                Assert.Equal(3, calledContexts.Count);
                Assert.Equal(input, calledContexts[0].Items);
                Assert.Equal(input, calledContexts[1].Items);
                Assert.Equal(input, calledContexts[2].Items);

                // Verify logging was not called as the filters don't filter anything
                _mockLogger.VerifyLogging(LogLevel.Debug, Times.Never());
            }

            [Fact]
            public async Task NoCascadeSkipAsync()
            {
                AddFilter(itemFilter: i => i.Status = FilterStatus.Skip);

                var users = CreateMany<IUser>();
                var input = users.Select(u => CreateTestMigrationItem(u)).ToImmutableArray();

                var result = await _runner.ExecuteAsync(input, Cancel);

                // Verify filtering doesn't cascaded skipped status unless the result explicitly asks for it.
                Assert.All(input, i =>
                {
                    Assert.Equal(MigrationManifestEntryStatus.Skipped, i.ManifestEntry.Status);
                    Assert.False(i.ManifestEntry.CascadeSkip);
                });
            }

            [Fact]
            public async Task CascadeSkipAsync()
            {
                AddFilter(itemFilter: i => i.Status = FilterStatus.CascadeSkip);

                var users = CreateMany<IUser>();
                var input = users.Select(u => CreateTestMigrationItem(u)).ToImmutableArray();

                var result = await _runner.ExecuteAsync(input, Cancel);

                // Verify filtering can set cascade skip.
                Assert.All(input, i =>
                {
                    Assert.Equal(MigrationManifestEntryStatus.Skipped, i.ManifestEntry.Status);
                    Assert.True(i.ManifestEntry.CascadeSkip);
                });
            }
        }

        #endregion
    }
}
